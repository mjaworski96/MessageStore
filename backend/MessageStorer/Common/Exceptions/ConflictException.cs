using System;

namespace Common.Exceptions
{
    public abstract class ConflictException : HttpException
    {
        public override int StatusCode => 409;
        public ConflictException(string message) : base(message)
        {
        }  
    }
    public class UsernameConflictException : ConflictException
    {
        public UsernameConflictException() : base(Lang.Lang.UsernameConflict)
        {
        }
    }
    public class EmailConflictException : ConflictException
    {
        public EmailConflictException() : base(Lang.Lang.EmailConflict)
        {
        }
    }

}
