using System;
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
using System.Windows.Threading;
using MahApps.Metro.Controls;

namespace JabberWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
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

            jabberClient.OnError += jabberClient_OnError;
            jabberClient.OnAuthenticate += jabberClient_OnAuthenticate;
            jabberClient.OnAuthError += jabberClient_OnAuthError;
            jabberClient.OnMessage += jabberClient_OnMessage;

            rosterManager.OnRosterBegin += rosterManager_OnRosterBegin;
            rosterManager.OnRosterItem += rosterManager_OnRosterItem;

            jabberClient.Connect();
        }

        void AddMsgToMsgList(string message)
        {
            TextBlock msgBox = new TextBlock();
            msgBox.Height = 32;
            msgBox.Text = message;

            messagesStackPanel.Children.Add(msgBox);
        }

        void jabberClient_OnMessage(object sender, Message msg)
        {
            if (!string.IsNullOrEmpty(msg.Body))
            {
                string m = string.Format("{0} : {1}", msg.From, msg.Body);
                messagesStackPanel.Dispatcher.Invoke((Action)(() => AddMsgToMsgList(m)));
            }
        }

        void rosterManager_OnRosterBegin(object sender)
        {
            rosterStackPanel.Dispatcher.Invoke((Action)(() => rosterStackPanel.Children.Clear()));
        }

        void AddUserToRosterList(string user)
        {
            TextBlock entry = new TextBlock();
            entry.Height = 32;
            entry.Text = user;

            rosterStackPanel.Children.Add(entry);
        }

        void rosterManager_OnRosterItem(object sender, Item ri)
        {
            string user = string.Format("{0} ({1})", ri.Nickname, ri.JID);

            rosterStackPanel.Dispatcher.Invoke((Action)(() => AddUserToRosterList(user)));
        }

        void jabberClient_OnAuthError(object sender, System.Xml.XmlElement rp)
        {
            string errorMsg = string.Format("Error:{0}", rp.InnerText);
            statusLabel.Dispatcher.Invoke((Action)(() => { statusLabel.Content = errorMsg; }));
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
            messagesStackPanel.Dispatcher.Invoke((Action)(() => AddMsgToMsgList(m)));
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
