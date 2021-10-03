using API.Config;
using API.Controllers;
using API.Dto;
using Common.Exceptions;
using API.Persistance.Entity;
using API.Persistance.Repository;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Common.Service;
using API.Infrastructure;
using Microsoft.Extensions.Logging;

namespace API.Service
{
    public interface IAppUserService
    {
        Task<AppUserDtoWithId> GetUser(int userId);
        Task<UserAndToken> Login(AppUserLoginDetails loginDetails);
        Task<UserAndToken> Register(AppUserRegisterDetails registerDetails);
        Task<UserAndToken> Modify(int userId, AppUserDto user);
        Task Remove(int userId);
        Task ChangePassword(int userId, AppUserPasswordChange password);
        Task<UserAndToken> Refresh(string oldToken);
        Task<InternalTokenDto> CreateInternalToken(int userId);
    }
    public class AppUserService : IAppUserService
    {
        private static readonly DateTime UnixEpochStart =
               DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
        private const string TokenPrefix = "Bearer ";

        private readonly IAppUserRepository _appUserRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly ISecurityService _securityService;
        private readonly IMessengerIntegrationClient _messengerIntegrationClient;
        private readonly ISecurityConfig _config;
        private readonly IHttpMetadataService _httpMetadataService;

        public AppUserService(IAppUserRepository appUserRepository,
            IAttachmentRepository attachmentRepository,
            IAttachmentService attachmentService,
            ISecurityService securityService,
            IMessengerIntegrationClient messengerIntegrationClient,
            ISecurityConfig config, 
            IHttpMetadataService httpMetadataService)
        {
            _appUserRepository = appUserRepository;
            _attachmentRepository = attachmentRepository;
            _attachmentService = attachmentService;
            _securityService = securityService;
            _messengerIntegrationClient = messengerIntegrationClient;
            _config = config;
            _httpMetadataService = httpMetadataService;
        }

        public async Task<AppUserDtoWithId> GetUser(int userId)
        {
            var userEntity = await _appUserRepository.Get(userId, true);
            return CreateAppUserDtoWithId(userEntity);
        }
        public async Task<UserAndToken> Login(AppUserLoginDetails loginDetails)
        {
            AppUsers user = await _appUserRepository.GetByUsername(loginDetails.Username, false);
            CheckCredentials(user, loginDetails);
            return CreateUserAndToken(user, false);
        }

        public async Task<UserAndToken> Register(AppUserRegisterDetails registerDetails)
        {
            ValidateUsername(registerDetails.Username);
            ValidatePassword(registerDetails.Password);
            ValidateEmail(registerDetails.Email);
            await CheckIfUserUnique(registerDetails.Username, registerDetails.Email);
            var userEntity = new AppUsers
            {
                Username = registerDetails.Username,
                Email = registerDetails.Email,
                Password = EncryptPassword(registerDetails.Password)
            };
            await _appUserRepository.Add(userEntity);
            return CreateUserAndToken(userEntity, false);
        }

        public async Task<UserAndToken> Modify(int userId, AppUserDto user)
        {
            ValidateUsername(user.Username);
            ValidateEmail(user.Email);
            var userEntity = await _appUserRepository.Get(userId, true);
            await CheckIfUserUnique(userEntity, user);
            userEntity.Username = user.Username;
            userEntity.Email = user.Email;
            await _appUserRepository.Save();
            return CreateUserAndToken(userEntity, false);
        }

        public async Task Remove(int userId)
        {
            _securityService.CheckIfUserCanDeleteAccount(userId);
            var userAttachments = await _attachmentRepository.GetAllAttachmentsFilenamesForUser(userId);
            await _appUserRepository.Remove(userId);
            await _messengerIntegrationClient.DeleteAllImports(userId);
            _attachmentService.DeleteAllAttachments(userId, userAttachments);
        }
        public async Task ChangePassword(int userId, AppUserPasswordChange password)
        {
            var user = await _appUserRepository.Get(userId, true);
            CheckPassword(user, password.OldPassword, true);
            ValidatePassword(password.NewPassword);
            user.Password = EncryptPassword(password.NewPassword);
            await _appUserRepository.Save();
        }
        public async Task<UserAndToken> Refresh(string oldToken)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(oldToken.Substring(TokenPrefix.Length));
            var issuedAt = jwtToken.Claims.FirstOrDefault(x => x.Type == "iat")?.Value;
            if (!string.IsNullOrEmpty(issuedAt))
            {
                var issuedAtDateTime = UnixEpochStart
                    .Add(TimeSpan.FromSeconds(int.Parse(issuedAt)));
                var refreshAfter = _httpMetadataService.InternalToken ? _config.InternalRefreshAfter : _config.RefreshAfter;
                if (issuedAtDateTime
                    .AddMinutes(refreshAfter)
                    .CompareTo(DateTime.UtcNow) < 0)
                {
                    var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var user = await _appUserRepository.Get(int.Parse(userId), true);
                        return CreateUserAndToken(user, _httpMetadataService.InternalToken);
                    }
                }
            }
            return null;
        }
        public async Task<InternalTokenDto> CreateInternalToken(int userId)
        {
            var authorization = _httpMetadataService.AuthorizationToken;
            if (string.IsNullOrEmpty(authorization))
            {
                throw new UnauthorizedAccessException();
            }
            if (authorization != _config.InternalToken)
            {
                throw new ForbiddenResourceException();
            }
            var appUser = await _appUserRepository.Get(userId, false);
            if (appUser == null)
            {
                throw new InvalidUsernameAndPasswordException();
            }

            return new InternalTokenDto
            {
                Token = GenerateToken(appUser, true)
            };      
        }
        private UserAndToken CreateUserAndToken(AppUsers userEntity, bool internalToken)
        {
            var appUser = CreateAppUserDtoWithId(userEntity);
            return new UserAndToken()
            {
                AppUser = appUser,
                Token = GenerateToken(userEntity, internalToken)
            };
        }
        private AppUserDtoWithId CreateAppUserDtoWithId(AppUsers user)
        {
            return new AppUserDtoWithId()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
            };
        }
        private void CheckCredentials(AppUsers appUser, AppUserLoginDetails credentials)
        {
            if (appUser == null)
            {
                throw new InvalidUsernameAndPasswordException();
            }
            CheckPassword(appUser, credentials.Password, false);
        }
        private void CheckPassword(AppUsers appUser, string password, bool loggedIn)
        {
            var passwordValid = BCrypt.Net.BCrypt.Verify(password, appUser.Password);
            if (!passwordValid)
            {
                if (loggedIn)
                {
                    throw new InvalidPasswordException();
                }
                else
                {
                    throw new InvalidUsernameAndPasswordException();
                }
            }
        }
        private string EncryptPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        private string GenerateToken(AppUsers user, bool internalToken)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var validFor = internalToken ? _config.InternalValidFor : _config.ValidFor;
            var header = new JwtHeader(credentials);
            var now = DateTime.UtcNow;
            var nowUnix = ((int)(now - UnixEpochStart).TotalSeconds).ToString();
            var payload = new JwtPayload(
                new Claim[]
                {
                    new Claim("sub", user.Id.ToString()),
                    new Claim("iat", nowUnix),
                    new Claim("nbf", nowUnix),
                    new Claim("exp", ((int)(now.AddMinutes(validFor) - UnixEpochStart).TotalSeconds).ToString()),
                    new Claim("int", internalToken.ToString())
                });

            var token = new JwtSecurityToken(header, payload);

            return $"{TokenPrefix}{new JwtSecurityTokenHandler().WriteToken(token)}";
        }
        private async Task CheckIfUserUnique(string username, string email)
        {
            if (await _appUserRepository.GetByUsername(username, false) != null)
            {
                throw new UsernameConflictException();
            }
            if (await _appUserRepository.GetByEmail(email, false) != null)
            {
                throw new EmailConflictException();
            }
        }
        private void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new EmptyUsernameException();
            }
            if (username.Length > 20)
            {
                throw new TooLongUsernameException();
            }
        }
        private void ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new EmptyPasswordException();
            }
        }
        private void ValidateEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    throw new InvalidEmailException();
                }
            }
            catch
            {
                throw new InvalidEmailException();
            }
        }
        private async Task CheckIfUserUnique(AppUsers currentData, AppUserDto newData)
        {
            if (currentData.Username != newData.Username)
            {
                if (await _appUserRepository.GetByUsername(newData.Username, false) != null)
                {
                    throw new UsernameConflictException();
                }
            }
            if (currentData.Email != newData.Email)
            {
                if (await _appUserRepository.GetByEmail(newData.Email, false) != null)
                {
                    throw new EmailConflictException();
                }
            }
        }
    }
}
