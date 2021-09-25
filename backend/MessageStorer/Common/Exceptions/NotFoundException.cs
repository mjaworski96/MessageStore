using Common.Lang;
using System;

namespace Common.Exceptions
{
    public abstract class NotFoundException : HttpException
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public override int StatusCode => 404;
    }
    public class ApplicationNotFoundException : NotFoundException
    {
        public ApplicationNotFoundException(string application, Exception innerException) :
            base(string.Format(Lang.Lang.ApplicationNotFound, application.Translate()), innerException)
        { }
    }
    public class UserByIdNotFoundException : NotFoundException
    {
        public UserByIdNotFoundException(int id, Exception innerException) :
            base(string.Format(Lang.Lang.UserByIdNotFound, id), innerException)
        { }
    }
    public class UserByUsernameNotFoundException : NotFoundException
    {
        public UserByUsernameNotFoundException(string username, Exception innerException) :
            base(string.Format(Lang.Lang.UserByUsernameNotFound, username), innerException)
        { }
    }
    public class UserByEmailException : NotFoundException
    {
        public UserByEmailException(string email, Exception innerException) :
            base(string.Format(Lang.Lang.UserByEmail, email), innerException)
        { }
    }
    public class AttachmentNotFoundException : NotFoundException
    {
        public AttachmentNotFoundException(int id, Exception innerException) :
           base(string.Format(Lang.Lang.AttachmentNotFound, id), innerException)
        { }
    }
    public class ContactNotFoundException : NotFoundException
    {
        public ContactNotFoundException(int id, Exception innerException) :
            base(string.Format(Lang.Lang.ContactNotFound, id), innerException)
        { }
    }
    public class WriterTypeNotFoundException : NotFoundException
    {
        public WriterTypeNotFoundException(string writerType, Exception innerException) :
           base(string.Format(Lang.Lang.WriterTypeNotFound, writerType.Translate()), innerException)
        { }
    }
    public class ContactsNotFoundException : NotFoundException
    {
        public ContactsNotFoundException() : base(Lang.Lang.ContactsNotFound)
        {
        }
    }
    public class ImportNotFoundException : NotFoundException
    {
        public ImportNotFoundException(string importId) :
            base(string.Format(Lang.Lang.ImportNotFound, importId))
        { }

        public ImportNotFoundException(string importId, Exception innerException) :
            base(string.Format(Lang.Lang.ImportNotFound, importId), innerException)
        { }
    }
    public class StatusNotFoundException : NotFoundException
    {
        public StatusNotFoundException(string status, Exception innerException) :
            base(string.Format(Lang.Lang.StatusNotFound, status), innerException) { }
    }
    public class AliasNotFoundException : NotFoundException
    {
        public AliasNotFoundException(int id, Exception innerException) :
            base(string.Format(Lang.Lang.AliasNotFound, id), innerException) { }
    }

}
