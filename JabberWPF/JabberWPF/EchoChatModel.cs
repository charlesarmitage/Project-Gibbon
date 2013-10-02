using System;
using System.Collections.Generic;
using jabber.client;

namespace JabberWPF
{
    internal class EchoChatModel : IChatModel
    {
        public EchoChatModel()
        {
            this.Configuration = new ClientConfig();
            this.Configuration.Username = "charles";
            this.Configuration.Password = "not in the clear?";
            this.Configuration.Server = "echoserver";
            this.Configuration.ServerName = "echo";

            this.Messages = new List<string>();
        }

        public ClientConfig Configuration { get; private set; }
        public JabberClient Client { get; private set; }
        public ICollection<string> Roster { get; private set; }
        public ICollection<string> Messages { get; private set; }
        public event Action<string, string> MessageTransmitted;
        public event Action<string, string> MessageReceived;
        public event Action<string> StatusUpdate;
        public event Action RosterChanged;

        public void SendMessage(string target, string text)
        {
            this.Messages.Add(string.Format("You: {0}", text));
            this.MessageTransmitted("You", text);
        }
    }
}