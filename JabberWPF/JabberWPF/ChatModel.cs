﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using jabber.client;
using jabber.protocol.client;
using jabber.protocol.iq;

namespace JabberWPF
{
    public class ChatModel
    {
        private readonly JabberClient _jabberClient = new JabberClient();
        private readonly RosterManager _rosterManager;
        private readonly Configuration _clientConfig;
        private readonly ClientConfig _config;
     
        public ChatModel()
        {
            _rosterManager = new RosterManager {Stream = _jabberClient};
            _clientConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _config = (ClientConfig)_clientConfig.Sections["clientconfig"];

            ApplyDefaultJabberConfiguration(_jabberClient);

            _jabberClient.OnError += jabberClient_OnError;
            _jabberClient.OnAuthenticate += jabberClient_OnAuthenticate;
            _jabberClient.OnAuthError += jabberClient_OnAuthError;
            _jabberClient.OnMessage += jabberClient_OnMessage;
            _rosterManager.OnRosterItem += rosterManager_OnRosterItem;

            _jabberClient.Connect();
        }

        public ClientConfig Configuration { get { return _config; } }

        public JabberClient Client { get { return _jabberClient; } }

        public event Action<string, string> MessageTransmitted;

        public event Action<string, string> MessageReceived;

        public event Action<string> StatusUpdate;

        public event Action RosterChanged;

        public void SendMessage(string target, string text)
        {
            var msg = new Message(_jabberClient.Document);
            msg.Body = text;

            foreach (jabber.JID rJid in _rosterManager)
            {
                Item rItem = _rosterManager[rJid];

                if (target == rJid.User)
                {
                    msg.To = rJid.Bare;
                    break;
                }

                if (target == rItem.LocalName)
                {
                    msg.To = rItem.JID;
                    break;
                }

                if (target == rJid.Bare)
                {
                    msg.To = rItem.JID;
                    break;
                }
            }

            _jabberClient.Write(msg);

            var m = string.Format("{0} : {1}", msg.To, msg.Body);
            OnMessageTransmitted("You", m);
        }

        protected virtual void OnMessageTransmitted(string arg1, string arg2)
        {
            var handler = MessageTransmitted;
            if (handler != null) handler(arg1, arg2);
        }

        private void ApplyDefaultJabberConfiguration(JabberClient jabberClient)
        {
            jabberClient.User = _config.Username;
            jabberClient.Password = _config.Password;
            jabberClient.NetworkHost = _config.Server;
            jabberClient.Server = _config.ServerName;
            jabberClient.AutoReconnect = 30F;
            jabberClient.AutoStartCompression = true;
            jabberClient.AutoStartTLS = true;
            jabberClient.KeepAlive = 30F;
            jabberClient.LocalCertificate = null;
        }

        void jabberClient_OnAuthError(object sender, System.Xml.XmlElement rp)
        {
            string errorMsg = string.Format("Error:{0}", rp.InnerText);
            OnStatusUpdate(errorMsg);
        }

        void jabberClient_OnAuthenticate(object sender)
        {
            _clientConfig.Save(ConfigurationSaveMode.Full);

            var statusMsg = string.Format("Online as : {0}", _jabberClient.User);
            OnStatusUpdate(statusMsg);
        }

        void jabberClient_OnError(object sender, Exception ex)
        {
            var errorMsg = string.Format("Error:{0}", ex.Message);
            OnStatusUpdate(errorMsg);
        }

        protected virtual void OnStatusUpdate(string obj)
        {
            Action<string> handler = StatusUpdate;
            if (handler != null) handler(obj);
        }

        void rosterManager_OnRosterItem(object sender, Item ri)
        {
            string user = string.Format("{0} ({1})", ri.Nickname, ri.JID);
            OnRosterChanged();
            //rosterStackPanel.Dispatcher.Invoke((Action)(() => AddUserToRosterList(user)));
        }

        protected virtual void OnRosterChanged()
        {
            var handler = RosterChanged;
            if (handler != null) handler();
        }

        void jabberClient_OnMessage(object sender, Message msg)
        {
            if (string.IsNullOrEmpty(msg.Body)) return;

            var m = string.Format("{0} : {1}", msg.From, msg.Body);
            OnMessageReceived(msg.From, m);
        }

        protected virtual void OnMessageReceived(string arg1, string arg2)
        {
            var handler = MessageReceived;
            if (handler != null) handler(arg1, arg2);
        }
    }
}