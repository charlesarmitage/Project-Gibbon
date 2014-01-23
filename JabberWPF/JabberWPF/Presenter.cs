using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace JabberWPF
{
    public class Presenter : ObserverableObject
    {
        private readonly IChatModel _chatModel;
        private string _status;
        private string _messageToSend;
        private string _errorMessage;
        private string _recipient;

        public Presenter()
        {
            Status = "Offline";
            Roster = new ObservableCollection<string>();
            Messages = new ObservableCollection<string>();
            MessageToSend = string.Empty;

            _chatModel = new ChatModel();//new EchoChatModel();
            ConnectToChatModel(_chatModel);
        }

        private void ConnectToChatModel(IChatModel chatModel)
        {
            chatModel.StatusUpdate += OnStatusUpdate;
            chatModel.ErrorMessage += OnErrorMessage;
            chatModel.RosterChanged += OnRosterChanged;
            chatModel.MessageReceived += OnMessageReceived;
            chatModel.MessageTransmitted += OnMessageTransmitted;
        }

        public string Status
        {
            get { return _status; }
            set 
            { 
                _status = value;
                RaisePropertyChangedEvent("Status");
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set 
            {
                _errorMessage = value;
                RaisePropertyChangedEvent("ErrorMessage");
            }
        }

        public ObservableCollection<string> Roster { get; private set; }

        public ObservableCollection<string> Messages { get; private set; }

        public string Recipient
        {
            get { return _recipient; }
            set
            {
                _recipient = value;
                RaisePropertyChangedEvent("Recipient");
            }
        }

        public string MessageToSend
        {
            get { return _messageToSend; }
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
                return new ChatCommand(ConfigureClient);
            }
        }

        public ICommand SendMessage
        {
            get
            {
                return new ChatCommand(s => TransmitMessage());
            }
        }

        private void OnStatusUpdate(string status)
        {
            Status = status;
            ErrorMessage = "";
        }

        private void OnErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        private void OnRosterChanged()
        {
            var newRosterItems = _chatModel.Roster.Except(Roster);
            AddToObservableCollection(Roster, newRosterItems);
        }

        private void OnMessageReceived(string sender, string message)
        {
            AddNewMessagesToMessageList(_chatModel.Messages);
        }

        private void OnMessageTransmitted(string sender, string message)
        {
            AddNewMessagesToMessageList(_chatModel.Messages);
        }

        public void UpdateSendMessageText(string text)
        {
            var recipient = GetMessageRecipient(text);
            Recipient = recipient.Trim('@');
        }

        public void SendMessageKeyPressed(Key key)
        {
            switch (key)
            {
                case Key.Enter:
                    TransmitMessage();
                    break;
                case Key.Escape:
                    MessageToSend = string.Empty;
                    break;
            }
        }

        private void AddNewMessagesToMessageList(IEnumerable<string> modelMessages)
        {
            var newMessages = modelMessages.Except(Messages);
            AddToObservableCollection(Messages, newMessages);
        }

        private void ConfigureClient(string obj)
        {
            var configWin = new ConfigurationWindow(_chatModel.Client, _chatModel.Configuration);
            configWin.ShowDialog();
        }

        private void TransmitMessage()
        {
            if(string.IsNullOrEmpty(MessageToSend))
            {
                return;
            }

            var recipient = GetMessageRecipient(MessageToSend);
            var messageText = MessageToSend.Replace(recipient, "");

            Recipient = recipient.Trim('@');
            _chatModel.SendMessage(Recipient, messageText);
            MessageToSend = string.Empty;
        }

        private static string GetMessageRecipient(string message)
        {
            var words = message.Split();
            return words.FirstOrDefault(word => word.StartsWith("@")) ?? string.Empty;
        }

        private static void AddToObservableCollection(ObservableCollection<string> observableCollection, IEnumerable<string> newItems)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                foreach (var newItem in newItems)
                {
                    observableCollection.Add(newItem);
                }
            }));
        }
    }
}