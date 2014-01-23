using System;
using System.Windows.Input;

namespace JabberWPF
{
    public class ChatCommand : ICommand
    {
        Action<string> _action;

        public ChatCommand(Action<string> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action(string.Empty);
        }
    }
}