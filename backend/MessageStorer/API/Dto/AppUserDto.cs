using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dto
{
    public class UserAndToken
    {
        public AppUserDtoWithId AppUser { get; set; }
        public string Token { get; set; }
    }
    public class AppUserDtoWithId
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
    public class AppUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
    public class AppUserPasswordChange
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class AppUserLoginDetails
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class AppUserRegisterDetails
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
