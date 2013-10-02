using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace JabberWPF
{
    public class Presenter : ObserverableObject
    {
        private ObservableCollection<string> _messages = new ObservableCollection<string>();
        private ObservableCollection<string> _roster = new ObservableCollection<string>();
        private string _messageToSend = string.Empty;
        private readonly ChatModel _chatModel =new ChatModel();

        public Presenter()
        {
            for (int i = 0; i < 50; i++)
            {
                _roster.Add("Laura");
                _roster.Add("Philippa");
            }
            this.Status = "Offline";
            _chatModel.StatusUpdate += OnStatusUpdate;
            _chatModel.RosterChanged += OnRosterChanged;
            _chatModel.MessageReceived += OnMessageReceived;
            _chatModel.MessageTransmitted += OnMessageTransmitted;
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

        private void sendMsgTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.TransmitMessage();
            }
            else if (e.Key == Key.Escape)
            {

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