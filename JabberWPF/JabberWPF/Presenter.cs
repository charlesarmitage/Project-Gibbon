using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

namespace JabberWPF
{
    public class Presenter : ObserverableObject
    {
        private ObservableCollection<string> _messages = new ObservableCollection<string>();
        private ObservableCollection<string> _roster = new ObservableCollection<string>();
        private string _messageToSend = string.Empty;
        private readonly IChatModel _chatModel;

        public Presenter()
        {
            for (int i = 0; i < 50; i++)
            {
                _roster.Add("Laura");
                _roster.Add("Philippa");
            }
            this.Status = "Offline";
            _chatModel = new EchoChatModel();
            this.ConnectToChatModel(this._chatModel);
        }

        private void ConnectToChatModel(IChatModel chatModel)
        {
            chatModel.StatusUpdate += this.OnStatusUpdate;
            chatModel.RosterChanged += this.OnRosterChanged;
            chatModel.MessageReceived += this.OnMessageReceived;
            chatModel.MessageTransmitted += this.OnMessageTransmitted;
        }

        public string Status { get; set; }

        public IEnumerable<string> Roster
        {
            get { return this._roster;  }
        }

        public IEnumerable<string> Messages
        {
            get
            {
                return _messages;
            }
        }

        public string Recipient{ get; set; }

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

        private void OnStatusUpdate(string status)
        {
            this.Status = status;
            RaisePropertyChangedEvent("Status");
        }

        private void OnRosterChanged()
        {
            this._roster = new ObservableCollection<string>(this._chatModel.Roster);
            RaisePropertyChangedEvent("Roster");
        }

        private void OnMessageReceived(string sender, string message)
        {
            this._messages = new ObservableCollection<string>(_chatModel.Messages);
            RaisePropertyChangedEvent("Messages");
        }

        private void OnMessageTransmitted(string arg1, string arg2)
        {
            this._messages = new ObservableCollection<string>(_chatModel.Messages);
            RaisePropertyChangedEvent("Messages");
        }

        public void UpdateSendMessageText(string text)
        {
            var words = text.Split();
            var recipient = words.FirstOrDefault(word => word.StartsWith(@"@"));
            if(recipient != null)
            {
                this.Recipient = recipient.Trim('@');
                RaisePropertyChangedEvent("Recipient");
            }
        }

        public void SendMessageKeyPressed(Key key)
        {
            switch (key)
            {
                case Key.Enter:
                    this.TransmitMessage();
                    break;
                case Key.Escape:
                    this.MessageToSend = string.Empty;
                    break;
            }
        }

        private void ConfgureClient(string obj)
        {
            var configWin = new ConfigurationWindow(_chatModel.Client, _chatModel.Configuration);
            configWin.ShowDialog();
        }

        private void Transmit(string message)
        {
            this.TransmitMessage();
        }

        private void TransmitMessage()
        {
            this._chatModel.SendMessage(this.Recipient, this._messageToSend);
            this.MessageToSend = string.Empty;
            RaisePropertyChangedEvent("Messages");
        }
    }
}