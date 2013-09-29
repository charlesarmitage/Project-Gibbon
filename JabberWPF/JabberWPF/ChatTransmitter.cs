using System;
using System.Windows.Input;

namespace JabberWPF
{
    public class ChatTransmitter : ICommand
    {
        Action<string> _action;

        public ChatTransmitter(Action<string> action)
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