using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace JabberWPF
{
    public class Presenter : ObserverableObject
    {
        private readonly MessageList messageList = new MessageList();
        private ObservableCollection<string> _messages = new ObservableCollection<string>();
        private ObservableCollection<string> _roster = new ObservableCollection<string>();
        private string _messageToSend = string.Empty;
        private readonly ChatModel _chatModel =new ChatModel();

        public Presenter()
        {
            _roster.Add("Laura");
            _roster.Add("Philippa");
            this.Status = "Offline";
            this.Recipient = "To...";
            _chatModel.StatusUpdate += OnStatusUpdate;
            _chatModel.RosterChanged += OnRosterChanged;
            _chatModel.MessageReceived += OnMessageReceived;
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

        private void sendMsgTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // SendMessage(sendMsgTextbox.Text);
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
            messageList.SendMessage(_messageToSend);
            _messages.Add(_messageToSend);
            _chatModel.SendMessage(this.Recipient, _messageToSend);

            RaisePropertyChangedEvent("Messages");
        }
    }
}