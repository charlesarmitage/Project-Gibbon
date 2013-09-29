using System.Collections.Generic;

namespace JabberWPF
{
    public class MessageList
    {
        public MessageList()
        {
            Messages = new List<string>();
        }

        public ICollection<string> Messages { get; private set; }

        public void SendMessage(string message)
        {
            Messages.Add(message);
        }
    }
}