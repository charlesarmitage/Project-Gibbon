using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
            this.Roster = new List<string>();

            for (int i = 0; i < 50; i++)
            {
                Roster.Add(string.Format("Laura - {0}", i));
                Roster.Add(string.Format("Philippa - {0}", i));
            }
            ThreadPool.QueueUserWorkItem( c =>
                                      {
                                          Thread.Sleep(5000);
                                          RosterChanged();
                                      });
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
            ThreadPool.QueueUserWorkItem(c => this.MessageTransmitted("You", text));
        }
    }
}