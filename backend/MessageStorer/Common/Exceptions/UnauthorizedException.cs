namespace Common.Exceptions
{
    public abstract class UnauthorizedException : HttpException
    {
        public override int StatusCode => 401;
        public UnauthorizedException(string message) : base(message)
        {
        }    
    }
    public class UserNotLoggedInException : UnauthorizedException
    {
        public UserNotLoggedInException(): base(Lang.Lang.UserNotLoggedInException)
        {
        }
    }
    public class InvalidUsernameAndPasswordException : UnauthorizedException
    {
        public InvalidUsernameAndPasswordException() : base(Lang.Lang.InvalidUsernameAndPassword)
        {
        }
    }
}
