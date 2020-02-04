using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
                CanExecuteChanged?.Invoke(null, new EventArgs());
            }
        }
        private readonly Func<Task> _action;
        private bool BlockWhileExecute;

        public DelegateCommand(Func<Task> action, bool executable = true, bool blockWhileExecute = false)
        {
            _action = action;
            Executable = executable;
            BlockWhileExecute = blockWhileExecute;
        }
        public bool CanExecute(object parameter)
        {
            return _action != null && Executable;
        }

        public void Execute(object parameter)
        {
            if (BlockWhileExecute)
                Executable = false;

            var task = _action();
            var tasks = new[] { task };
            Task.WaitAll(tasks);

            if (BlockWhileExecute)
                Executable = true;
        }
    }
}
