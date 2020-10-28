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
            var loginResponse = new AppUserDto()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
            };
            return new UserAndToken()
            {
                AppUser = loginResponse,
                Token = GenerateToken(user)
            };
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
    }
}
