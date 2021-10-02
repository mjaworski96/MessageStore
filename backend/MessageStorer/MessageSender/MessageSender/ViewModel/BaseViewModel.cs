using MessageSender.Model;
using MessageSender.Model.Http;
using System;
using System.ComponentModel;

namespace MessageSender.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ServerIp
        {
            get
            {
                return Constraints.URL;
            }
        }

        protected string CreateErrorMessage(ApiException apiException)
        {
            return apiException.Message;
        }

        protected string CreateErrorMessage(Exception exception)
        {
            return $"{exception.GetType().Name}\n{exception.Message}\n{exception.StackTrace}";
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
