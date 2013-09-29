using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace JabberWPF
{
    public class Presenter : ObserverableObject
    {
        private MessageList messageList = new MessageList();
        private ObservableCollection<string> _messages = new ObservableCollection<string>();
        private string _messageToSend = string.Empty;

        public Presenter()
        {
        }

        public IEnumerable<string> Messages
        {
            get
            {
                return _messages;
            }
        }

        public string MessageToSend
        {
            get
            {
                return _messageToSend;
            }
            set
            {
                _messageToSend = value;
                RaisePropertyChangedEvent("MessageToSend");
            }
        }

        public ICommand SendMessage
        {
            get
            {
                return new ChatTransmitter(Transmit);
            }
        }

        private void Transmit(string message)
        {
            messageList.SendMessage(_messageToSend);
            _messages.Add(_messageToSend);
            RaisePropertyChangedEvent("Messages");
        }
    }
}