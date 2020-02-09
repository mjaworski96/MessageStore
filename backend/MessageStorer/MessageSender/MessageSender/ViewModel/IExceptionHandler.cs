using System;

namespace MessageSender.ViewModel
{
    public interface IExceptionHandler
    {
        void Handle(Exception e);
        void Clear();
    }
}