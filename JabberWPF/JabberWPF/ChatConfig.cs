using System.Xml.Serialization;

namespace JabberWPF
{
    [System.SerializableAttribute]
    [XmlType]
    [XmlRoot]
    public class ChatConfig
    {
        [XmlElement("username", typeof(string))]
        public string Username { get; set; }

        [XmlElement("password", typeof(string))]
        public string Password { get; set; }

        [XmlElement("server", typeof(string))]
        public string Server { get; set; }

        [XmlElement("servername", typeof(string))]
        public string ServerName { get; set; }
    }
}