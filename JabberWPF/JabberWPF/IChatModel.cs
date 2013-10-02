using System;
using System.Collections.Generic;
using jabber.client;

namespace JabberWPF
{
    public interface IChatModel
    {
        ClientConfig Configuration { get; }
        JabberClient Client { get; }
        ICollection<string> Roster { get; }
        ICollection<string> Messages { get; }
        event Action<string, string> MessageTransmitted;
        event Action<string, string> MessageReceived;
        event Action<string> StatusUpdate;
        event Action RosterChanged;
        void SendMessage(string target, string text);
    }
}