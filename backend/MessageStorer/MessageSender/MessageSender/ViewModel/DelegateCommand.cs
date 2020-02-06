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

        private readonly Func<Task> _action;
        private bool _blockWhileExecute;

        public DelegateCommand(Func<Task> action, bool executable = true, bool blockWhileExecute = false)
        {
            _action = action;
            Executable = executable;
            _blockWhileExecute = blockWhileExecute;
        }
        public bool CanExecute(object parameter)
        {
            return _action != null && Executable;
        }

        public void Execute(object parameter)
        {
            Task.Run(async () => await Perfrom());
        }

        private async Task Perfrom()
        {
            if (_blockWhileExecute)
                Executable = false;

            await _action();

            if (_blockWhileExecute)
                Executable = true;
        }
    }
}
