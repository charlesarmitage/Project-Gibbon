using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using jabber.client;
using MahApps.Metro.Controls;

namespace JabberWPF
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : MetroWindow
    {
        private ChatConfig _chatConfig = new ChatConfig();
        JabberClient jabberClient;

        public ConfigurationWindow(JabberClient jClient, ClientConfig config)
        {
            InitializeComponent();
            var chatConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "gibbon.config");
            if(File.Exists(chatConfigPath))
            {
                ReadChatConfig(chatConfigPath);
            }
            else
            {
                ReadAppConfig(config);
            }

            jabberClient = jClient;

            userNameTextbox.Text = _chatConfig.Username;
            userPasswordBox.Text = _chatConfig.Password;
            serverTextbox.Text = _chatConfig.Server;
            serverNameTextbox.Text = _chatConfig.ServerName;

            jabberClient.OnAuthenticate += new bedrock.ObjectHandler(jabberClient_OnAuthenticate);
            jabberClient.OnAuthError += new jabber.protocol.ProtocolHandler(jabberClient_OnAuthError);
            jabberClient.OnError += new bedrock.ExceptionHandler(jabberClient_OnError);
        }

        private void ReadAppConfig(ClientConfig jabberConfig)
        {
            _chatConfig.Username = jabberConfig.Username;
            _chatConfig.Password = jabberConfig.Password;
            _chatConfig.Server = jabberConfig.Server;
            _chatConfig.ServerName = jabberConfig.ServerName;
        }

        private void ReadChatConfig(string chatConfigPath)
        {
            using (var configStream = new StreamReader(chatConfigPath))
            {
                var configXml = new XmlSerializer(typeof(ChatConfig));
                _chatConfig = (ChatConfig)configXml.Deserialize(configStream);
                configStream.Close();
            }
        }

        void jabberClient_OnError(object sender, Exception ex)
        {
            statusLabel.Dispatcher.Invoke((Action)(() => { statusLabel.Content = ex.Message; }));
        }

        void jabberClient_OnAuthError(object sender, System.Xml.XmlElement rp)
        {
            statusLabel.Dispatcher.Invoke((Action)(() => { statusLabel.Content = rp.InnerText; }));
        }

        void jabberClient_OnAuthenticate(object sender)
        {
            this.Dispatcher.Invoke((Action)(() => { Close(); }));
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() => { Close(); }));
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            _chatConfig.Username = this.userNameTextbox.Text;
            _chatConfig.Password = this.userPasswordBox.Text;
            _chatConfig.Server = this.serverTextbox.Text;
            _chatConfig.ServerName = this.serverNameTextbox.Text;

            ApplyJabberUserConfiguration(jabberClient);

            jabberClient.Connect();
        }

        private void ApplyJabberUserConfiguration(JabberClient jabberClient)
        {
            jabberClient.User = _chatConfig.Username;
            jabberClient.Password = _chatConfig.Password;
            jabberClient.NetworkHost = _chatConfig.Server;
            jabberClient.Server = _chatConfig.ServerName;
        }
    }
}
