using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using jabber.client;
using muzzle;
using System.Configuration;
using jabber.protocol.client;
using jabber.protocol.iq;

namespace JabberWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private JabberClient jabberClient;
        private RosterManager rosterManager;
        private Configuration clientConfig;
        private ClientConfig config;

        public MainWindow()
        {
            InitializeComponent();

            jabberClient = new JabberClient();
            rosterManager = new RosterManager();
            rosterManager.Stream = jabberClient;

            clientConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config = (ClientConfig)clientConfig.Sections["clientconfig"];

            ApplyDefaultJabberConfiguration( jabberClient );

            jabberClient.OnError += new bedrock.ExceptionHandler(jabberClient_OnError);
            jabberClient.OnAuthenticate += new bedrock.ObjectHandler(jabberClient_OnAuthenticate);
            jabberClient.OnAuthError += new jabber.protocol.ProtocolHandler(jabberClient_OnAuthError);
            jabberClient.OnMessage += new MessageHandler(jabberClient_OnMessage);

            rosterManager.OnRosterBegin += new bedrock.ObjectHandler(rosterManager_OnRosterBegin);
            rosterManager.OnRosterItem += new RosterItemHandler(rosterManager_OnRosterItem);

            jabberClient.Connect();
        }

        void AddMsgToMsgList(string message)
        {
            TextBlock msgBox = new TextBlock();
            msgBox.Height = 32;
            msgBox.Foreground = Brushes.White;
            msgBox.Text = message;

            messagesStackPanel.Children.Add(msgBox);
        }

        void jabberClient_OnMessage(object sender, jabber.protocol.client.Message msg)
        {
            if (!string.IsNullOrEmpty(msg.Body))
            {
                string m = string.Format("{0} : {1}", msg.From, msg.Body);
                messagesStackPanel.Dispatcher.Invoke((Action<string>)AddMsgToMsgList, m);
            }
        }

        void rosterManager_OnRosterBegin(object sender)
        {
            rosterStackPanel.Dispatcher.Invoke((Action)(() => { rosterStackPanel.Children.Clear(); }));
        }

        void AddUserToRosterList(string user)
        {
            TextBlock entry = new TextBlock();
            entry.Height = 32;
            entry.Foreground = Brushes.White;
            entry.Text = user;

            rosterStackPanel.Children.Add(entry);
        }

        void rosterManager_OnRosterItem(object sender, jabber.protocol.iq.Item ri)
        {
            string user = string.Format("{0} ({1})", ri.Nickname, ri.JID);

            rosterStackPanel.Dispatcher.Invoke((Action<string>)AddUserToRosterList, user);
        }

        void jabberClient_OnAuthError(object sender, System.Xml.XmlElement rp)
        {
            string errorMsg = string.Format("Error:{0}", rp.InnerText);
            Delegate errorStatus = (Action)(() => { statusLabel.Content = errorMsg; });

            statusLabel.Dispatcher.Invoke(errorStatus);
        }

        void jabberClient_OnAuthenticate(object sender)
        {
            this.clientConfig.Save(ConfigurationSaveMode.Full);

            string statusMsg = string.Format("Online as : {0}", jabberClient.User);
            Delegate updateStatus = (Action)(() => { statusLabel.Content = statusMsg; });
            statusLabel.Dispatcher.Invoke( updateStatus );
        }

        void jabberClient_OnError(object sender, Exception ex)
        {
            string errorMsg = string.Format("Error:{0}", ex.Message);
            Delegate errorStatus = (Action)(() => { statusLabel.Content = errorMsg; });

            statusLabel.Dispatcher.Invoke(errorStatus);
        }

        private void ApplyDefaultJabberConfiguration(JabberClient jabberClient)
        {
            ApplyJabberUserConfiguration(jabberClient);

            jabberClient.AutoReconnect = 30F;
            jabberClient.AutoStartCompression = true;
            jabberClient.AutoStartTLS = true;
            jabberClient.KeepAlive = 30F;
            jabberClient.LocalCertificate = null;
        }

        private void ApplyJabberUserConfiguration(JabberClient jabberClient)
        {
            jabberClient.User = config.Username;
            jabberClient.Password = config.Password;
            jabberClient.NetworkHost = config.Server;
            jabberClient.Server = config.ServerName;
        }

        private void configureButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationWindow configWin = new ConfigurationWindow(jabberClient, config);
            configWin.ShowDialog();
        }

        private void sendMsgTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage(sendMsgTextbox.Text);
            }
            else if (e.Key == Key.Escape)
            {

            }
        }

        private void SendMessage(string text)
        {
            Message msg = new Message(this.jabberClient.Document);
            msg.Body = text;

            foreach (jabber.JID rJID in this.rosterManager)
            {
                Item rItem = this.rosterManager[rJID];

                if (this.toTextbox.Text == rJID.User)
                {
                    msg.To = rJID.Bare;
                    break;
                }

                if (this.toTextbox.Text == rItem.LocalName)
                {
                    msg.To = rItem.JID;
                    break;
                }

                if (this.toTextbox.Text == rJID.Bare)
                {
                    msg.To = rItem.JID;
                    break;
                }
            }

            jabberClient.Write(msg);

            string m = string.Format("{0} : {1}", msg.To, msg.Body);
            messagesStackPanel.Dispatcher.Invoke((Action<string>)AddMsgToMsgList, m);
        }
    }
}
