﻿using System;
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

        public Presenter()
        {
            Status = "Offline";
            Roster = new ObservableCollection<string>();
            Messages = new ObservableCollection<string>();
            MessageToSend = string.Empty;

            _chatModel = new EchoChatModel();
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

        public string Status { get; set; }

        public string ErrorMessage { get; set; }

        public ObservableCollection<string> Roster { get; private set; }

        public ObservableCollection<string> Messages { get; private set; }

        public string Recipient { get; set; }

        public string MessageToSend { get; set; }

        public ICommand Configure
        {
            get
            {
                return new ChatTransmitter(ConfigureClient);
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
            Status = status;
        }

        private void OnErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;
            RaisePropertyChangedEvent("ErrorMessage");
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
            RaisePropertyChangedEvent("Recipient");
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

        private void Transmit(string message)
        {
            TransmitMessage();
        }

        private void TransmitMessage()
        {
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