using System;
using System.Windows.Input;

namespace JabberWPF
{
    class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public DelegateCommand(Action<object> executableAction, Predicate<object> canExecutePredicate = null)
        {
            _execute = executableAction;
            _canExecute = canExecutePredicate;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            if(_canExecute == null)
            {
                return true;
            }
            return _canExecute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if(_canExecute != null)
            {
                var eventHandler = CanExecuteChanged;
                if(eventHandler != null)
                {
                    eventHandler(this, EventArgs.Empty);
                }
            }
        }
    }
}
