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
        private readonly ISecurityConfig _config;
        private readonly IHttpMetadataService _httpMetadataService;

        public AppUserService(IAppUserRepository appUserRepository,
            ISecurityConfig config, IHttpMetadataService httpMetadataService)
        {
            _appUserRepository = appUserRepository;
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
            await _appUserRepository.Remove(userId);
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
            var expires = jwtToken.Claims.FirstOrDefault(x => x.Type == "exp")?.Value;
            if (!string.IsNullOrEmpty(expires))
            {
                var expirationTime = UnixEpochStart
                    .Add(TimeSpan.FromSeconds(int.Parse(expires)));
                var refreshBefore = _httpMetadataService.InternalToken ? _config.InternalRefreshBefore : _config.RefreshBefore;
                if (expirationTime
                    .AddMinutes(-refreshBefore)
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
                throw new UnauthorizedException("You are not authorized");
            }
            if (authorization != _config.InternalToken)
            {
                throw new ForbiddenException("You can't acces this resource");
            }
            var appUser = await _appUserRepository.Get(userId, false);
            if (appUser == null)
            {
                throw new UnauthorizedException("Invalid username or password");
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
                throw new UnauthorizedException("Invalid username or password");
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
                    throw new ForbiddenException("Password is invalid");
                }
                else
                {
                    throw new UnauthorizedException("Invalid username or password");
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
            var payload = new JwtPayload(
                new Claim[]
                {
                    new Claim("sub", user.Id.ToString()),
                    new Claim("exp",
                    ((int)(DateTime.UtcNow.AddMinutes(validFor) - UnixEpochStart).TotalSeconds).ToString()),
                    new Claim("int", internalToken.ToString())
                });

            var token = new JwtSecurityToken(header, payload);

            return $"{TokenPrefix}{new JwtSecurityTokenHandler().WriteToken(token)}";
        }
        private async Task CheckIfUserUnique(string username, string email)
        {
            if (await _appUserRepository.GetByUsername(username, false) != null)
            {
                throw new ConflictException("User with this username exists.");
            }
            if (await _appUserRepository.GetByEmail(email, false) != null)
            {
                throw new ConflictException("User with this email exists.");
            }
        }
        private void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new BadRequestException("Username can't be empty.");
            }
            if (username.Length > 20)
            {
                throw new BadRequestException("Username can't be longer than 20 characters.");
            }
        }
        private void ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new BadRequestException("Password can't be empty");
            }
        }
        private void ValidateEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    throw new BadRequestException("Invalid email");
                }
            }
            catch
            {
                throw new BadRequestException("Invalid email");
            }
        }
        private async Task CheckIfUserUnique(AppUsers currentData, AppUserDto newData)
        {
            if (currentData.Username != newData.Username)
            {
                if (await _appUserRepository.GetByUsername(newData.Username, false) != null)
                {
                    throw new ConflictException("User with this username exists.");
                }
            }
            if (currentData.Email != newData.Email)
            {
                if (await _appUserRepository.GetByEmail(newData.Email, false) != null)
                {
                    throw new ConflictException("User with this email exists.");
                }
            }
        }
    }
}
