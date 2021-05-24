using Common.Lang;

namespace Common.Exceptions
{
    public abstract class BadRequestException : HttpException
    {
        public override int StatusCode => 400;
        public BadRequestException(string message) : base(message)
        {
        } 
    }
    public class AliasCreationException : BadRequestException
    {
        public AliasCreationException() : base(Lang.Lang.AliasCreation)
        {
        }
    }
    public class RawAliasModificationException : BadRequestException
    {
        public RawAliasModificationException() : base(Lang.Lang.RawAliasModification)
        {
        }
    }
    public class RawAliasDeletionException : BadRequestException
    {
        public RawAliasDeletionException() : base(Lang.Lang.RawAliasDeletion)
        {
        }
    }
    public class InvalidAliasNameException : BadRequestException
    {
        public InvalidAliasNameException() : base(Lang.Lang.InvalidAliasName)
        {
        }
    }
    public class EmptyUsernameException : BadRequestException
    {
        public EmptyUsernameException() : base(Lang.Lang.EmptyUsername)
        {
        }
    }
    public class TooLongUsernameException : BadRequestException
    {
        public TooLongUsernameException() : base(Lang.Lang.TooLongUsername)
        {
        }
    }
    public class EmptyPasswordException : BadRequestException
    {
        public EmptyPasswordException() : base(Lang.Lang.EmptyPassword)
        {
        }
    }
    public class InvalidEmailException : BadRequestException
    {
        public InvalidEmailException() : base(Lang.Lang.InvalidEmail)
        {
        }
    }
    public class TooLongContactNameException : BadRequestException
    {
        public TooLongContactNameException() : base(Lang.Lang.TooLongContactName)
        {
        }
    }
    public class TooLongContactInApplicationIdException : BadRequestException
    {
        public TooLongContactInApplicationIdException() : base(Lang.Lang.TooLongContactInApplicationId)
        {
        }
    }
    public class TooLongContactMemberNameException : BadRequestException
    {
        public TooLongContactMemberNameException() : base(Lang.Lang.TooLongContactMemberName)
        {
        }
    }
    public class TooLongMessageContentException : BadRequestException
    {
        public TooLongMessageContentException() : base(Lang.Lang.TooLongMessageContent)
        {
        }
    }
    public class AppUserContactMemberException : BadRequestException
    {
        public AppUserContactMemberException() : base(Lang.Lang.AppUserContactMember)
        {
        }
    }
    public class MissingContactMemberException : BadRequestException
    {
        public MissingContactMemberException() : base(Lang.Lang.MissingContactMember)
        {
        }
    }
    public class EmptyFacebookNameException : BadRequestException
    {
        public EmptyFacebookNameException() : base(Lang.Lang.EmptyFacebookName)
        {
        }
    }
    public class TooLongFacebookNameException : BadRequestException
    {
        public TooLongFacebookNameException() : base(Lang.Lang.TooLongFacebookName)
        {
        }
    }
    public class InvalidImportStatusException : BadRequestException
    {
        public InvalidImportStatusException(string validStatus, string currentStatus) : 
            base (string.Format(Lang.Lang.InvalidImportStatus, validStatus.Translate(), currentStatus.Translate())) { }
    }
}
