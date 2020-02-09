using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;

namespace MessageSender.ViewModel
{
    public class DelegateCommand: ICommand
    {
        public event EventHandler CanExecuteChanged;
        private bool _executable;
        private readonly Func<Task> _action;
        private bool _blockWhileExecute;
        private readonly IExceptionHandler _exceptionHandler;

        public bool Executable
        {
            get
            {
                return _executable;
            }
            set
            {
                _executable = value;
                if (MainThread.IsMainThread)
                {
                    RaiseExecutableChanged();
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(RaiseExecutableChanged);
                } 
            }
        }

        private void RaiseExecutableChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public DelegateCommand(Func<Task> action, IExceptionHandler exceptionHandler, 
            bool executable, bool blockWhileExecute)
        {
            _action = action;
            Executable = executable;
            _blockWhileExecute = blockWhileExecute;
            _exceptionHandler = exceptionHandler;
        }
        public bool CanExecute(object parameter)
        {
            return _action != null && Executable;
        }

        public void Execute(object parameter)
        {
            _exceptionHandler?.Clear();
            Task.Run(async () => await Perfrom());
        }

        private async Task Perfrom()
        {
            if (_blockWhileExecute)
                Executable = false;

            try
            {
                await _action();
            }
            catch(Exception e)
            {
                _exceptionHandler?.Handle(e);
            }

            if (_blockWhileExecute)
                Executable = true;
        }
    }
}
