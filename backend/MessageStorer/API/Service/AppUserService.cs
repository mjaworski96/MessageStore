using API.Dto;
using API.Exceptions;
using API.Persistance.Entity;
using API.Persistance.Repository;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Service
{
    public interface IAppUserService
    {
        Task<UserAndToken> Login(AppUserLoginDetails loginDetails);
        Task<UserAndToken> Register(AppUserRegisterDetails registerDetails);
        Task<UserAndToken> Modify(string username, AppUserDto user);
        Task Remove(string username);
        Task ChangePassword(AppUserPasswordChange password);
    }
    public class AppUserService : IAppUserService
    {
        private static readonly DateTime UnixEpochStart =
               DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);

        private readonly IAppUserRepository _appUserRepository;
        private readonly ISecurityConfig _config;

        public AppUserService(IAppUserRepository appUserRepository,
            ISecurityConfig config)
        {
            _appUserRepository = appUserRepository;
            _config = config;
        }


        public async Task<UserAndToken> Login(AppUserLoginDetails loginDetails)
        {
            var user = await _appUserRepository.Get(loginDetails.Username);
            CheckCredentials(user, loginDetails);
            var loginResponse = GetAppUserDtoWithId(user);
            return new UserAndToken()
            {
                AppUser = loginResponse,
                Token = GenerateToken(user)
            };
        }

        private static AppUserDtoWithId GetAppUserDtoWithId(AppUsers user)
        {
            return new AppUserDtoWithId()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
            };
        }

        public async Task<UserAndToken> Register(AppUserRegisterDetails registerDetails)
        {
            ValidateEmail(registerDetails.Email);
            await CheckIfUserUnique(registerDetails.Username, registerDetails.Email);
            var userEntity = new AppUsers
            {
                Username = registerDetails.Username,
                Email = registerDetails.Email,
                Password = EncryptPassword(registerDetails.Password)
            };
            await _appUserRepository.Add(userEntity);
            var appUser = GetAppUserDtoWithId(userEntity);
            return new UserAndToken()
            {
                AppUser = appUser,
                Token = GenerateToken(userEntity)
            };
        }

        public Task<UserAndToken> Modify(string username, AppUserDto user)
        {
            throw new NotImplementedException();
        }

        public Task Remove(string username)
        {
            throw new NotImplementedException();
        }
        public Task ChangePassword(AppUserPasswordChange password)
        {
            throw new NotImplementedException();
        }
        private void CheckCredentials(AppUsers appUser, AppUserLoginDetails credentials)
        {
            if (appUser == null)
            {
                throw new UnauthorizedException("Invalid username or password");
            }
            var passwordValid = BCrypt.Net.BCrypt.Verify(credentials.Password, appUser.Password);
            if (!passwordValid)
            {
                throw new UnauthorizedException("Invalid username or password");
            }
        }
        private string EncryptPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        private string GenerateToken(AppUsers appUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var header = new JwtHeader(credentials);
            var payload = new JwtPayload(
                new Claim[]
                {
                    new Claim("sub", appUser.Username),
                    new Claim("exp",
                    ((int)(DateTime.UtcNow.AddMinutes(_config.ValidFor) - UnixEpochStart).TotalSeconds).ToString()),
                });

            var token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task CheckIfUserUnique(string username, string email)
        {
            try
            {
                await _appUserRepository.Get(username);
                throw new ConflictException("User with this username exists.");
            }
            catch (NotFoundException) { }
            try
            {
                await _appUserRepository.GetByEmail(email);
                throw new ConflictException("User with this email exists.");
            }
            catch (NotFoundException) { }
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
    }
}
