using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace JabberWPF
{
    public class ClientConfig : ConfigurationSection
    {
        private ConfigurationProperty username;
        private ConfigurationProperty password;
        private ConfigurationProperty server;
        private ConfigurationProperty serverName;

        public ClientConfig()
        {
            username = new ConfigurationProperty( "username", typeof(string), "chazza", ConfigurationPropertyOptions.IsRequired );
            password = new ConfigurationProperty("password", typeof(string), "chazza", ConfigurationPropertyOptions.IsRequired);
            server = new ConfigurationProperty("server", typeof(string), "127.0.0.1", ConfigurationPropertyOptions.IsRequired);
            serverName = new ConfigurationProperty("serverName", typeof(string), "127.0.0.1", ConfigurationPropertyOptions.IsRequired);
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        [ConfigurationProperty("username", IsRequired = true)]
        public string Username
        {
            get { return (string)base["username"]; }
            set { base["username"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return (string)base["password"]; }
            set { base["password"] = value; }
        }

        [ConfigurationProperty("server", IsRequired = true)]
        public string Server
        {
            get { return (string)base["server"]; }
            set { base["server"] = value; }
        }

        [ConfigurationProperty("serverName", IsRequired = true)]
        public string ServerName
        {
            get { return (string)base["serverName"]; }
            set { base["serverName"] = value; }
        }
    }
}
