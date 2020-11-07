using MessageSender.Model;
using MessageSender.Model.Http;
using MessageSender.ViewModel.Interfaces;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MessageSender.ViewModel
{
    public class LoginPageViewModel : BaseViewModel, INotifyPropertyChanged, IExceptionHandler
    {
        private string _error;
        private string _username;
        private string _password;

        private IPageChanger _pageChanger;

        public LoginPageViewModel(IPageChanger pageChanger)
        {
            _pageChanger = pageChanger;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Error
        {
            get => _error;
            set
            {
                _error = value;
                NotifyPropertyChanged("Error");
            }
        }
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                NotifyPropertyChanged("Username");
                NotifyPropertyChanged("LoginCommand");
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                NotifyPropertyChanged("Password");
                NotifyPropertyChanged("LoginCommand");
            }
        }
        public ICommand LoginCommand { get => new DelegateCommand(Login, this, CanLogIn(), true); }

        public void Handle(Exception e)
        {
            if (e is ApiException apiException)
            {
                Error = $"{apiException.Code} - {apiException.Message}";
            }
            else
            {
                Error = $"{e.GetType().Name}\n{e.Message}\n{e.StackTrace}";
            }
        }
        public void Clear()
        {
            Error = "";
        }    
        private bool CanLogIn()
        {
            return (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));
        }
        private async Task Login()
        {
            using(var userHttpSender = new UserHttpSender(Config.ServerAddress))
            {
                var loginDetails = new LoginDetails
                {
                    Username = Username,
                    Password = Password
                };
                await userHttpSender.Login(loginDetails);
                _pageChanger.ShowMainPage();
            }
        }
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
