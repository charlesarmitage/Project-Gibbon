using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Input;
using jabber.client;

namespace JabberWPF
{
    public class Presenter : ObserverableObject
    {
        private MessageList messageList = new MessageList();
        private ObservableCollection<string> _messages = new ObservableCollection<string>();
        private ObservableCollection<string> _roster = new ObservableCollection<string>();
        private string _messageToSend = string.Empty;
        private ChatModel _chatModel =new ChatModel();


        public Presenter()
        {
            _roster.Add("Laura");
            _roster.Add("Philippa");
        }

        public string Status
        {
            get { return "Offline"; }
        }

        public IEnumerable<string> Roster
        {
            get { return _roster;  }
        }

        public IEnumerable<string> Messages
        {
            get
            {
                return _messages;
            }
        }

        public string Recipient
        {
            get { return "To..."; }
            set { }
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

        public ICommand Configure
        {
            get
            {
                return new ChatTransmitter(ConfgureClient);
            }
        }

        public ICommand SendMessage
        {
            get
            {
                return new ChatTransmitter(Transmit);
            }
        }

        private void ConfgureClient(string obj)
        {
            var configWin = new ConfigurationWindow(_chatModel.Client, _chatModel.Configuration);
            configWin.ShowDialog();
        }

        private void Transmit(string message)
        {
            messageList.SendMessage(_messageToSend);
            _messages.Add(_messageToSend);
            RaisePropertyChangedEvent("Messages");
        }
    }
}