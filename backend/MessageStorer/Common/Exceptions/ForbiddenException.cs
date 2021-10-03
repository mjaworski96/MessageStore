using System;

namespace Common.Exceptions
{
    public abstract class ForbiddenException : HttpException
    {
        public override int StatusCode => 403;

        public ForbiddenException(string message) : base(message)
        {
        }   
    }
    public class ForbiddenResourceException : ForbiddenException
    {
        public ForbiddenResourceException() : base(Lang.Lang.ForbiddenResource)
        {
        }
    }
    public class InvalidPasswordException : ForbiddenException
    {
        public InvalidPasswordException() : base(Lang.Lang.InvalidPassword)
        {
        }
    }
    public class ForbiddenAliasException : ForbiddenException
    {
        public ForbiddenAliasException() : base(Lang.Lang.ForbiddenAlias)
        {
        }
    }
    public class ForbiddenContactException : ForbiddenException
    {
        public ForbiddenContactException() : base(Lang.Lang.ForbiddenContact)
        {
        }
    }
    public class ForbiddenAttachmentException : ForbiddenException
    {
        public ForbiddenAttachmentException() : base(Lang.Lang.ForbiddenAttachment)
        {
        }
    }
    public class ForbiddenImportException : ForbiddenException
    {
        public ForbiddenImportException() : base(Lang.Lang.ForbiddenImport)
        {
        }
    }
    public class ForbiddenUserDeleteException: ForbiddenException
    {
        public ForbiddenUserDeleteException() : base(Lang.Lang.ForbiddenUserDelete)
        {
        }
    }
}
