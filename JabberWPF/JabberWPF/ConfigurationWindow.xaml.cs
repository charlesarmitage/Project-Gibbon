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
using System.Windows.Shapes;
using jabber.client;

namespace JabberWPF
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        ClientConfig jabberConfig;
        JabberClient jabberClient;

        public ConfigurationWindow(JabberClient jClient, ClientConfig config)
        {
            InitializeComponent();

            jabberConfig = config;
            jabberClient = jClient;

            this.userNameTextbox.Text = this.jabberConfig.Username;
            this.userPasswordBox.Text = this.jabberConfig.Password;
            this.serverTextbox.Text = this.jabberConfig.Server;
            this.serverNameTextbox.Text = this.jabberConfig.ServerName;

            jabberClient.OnAuthenticate += new bedrock.ObjectHandler(jabberClient_OnAuthenticate);
            jabberClient.OnAuthError += new jabber.protocol.ProtocolHandler(jabberClient_OnAuthError);
            jabberClient.OnError += new bedrock.ExceptionHandler(jabberClient_OnError);
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
            this.jabberConfig.Username = this.userNameTextbox.Text;
            this.jabberConfig.Password = this.userPasswordBox.Text;
            this.jabberConfig.Server = this.serverTextbox.Text;
            this.jabberConfig.ServerName = this.serverNameTextbox.Text;

            ApplyJabberUserConfiguration(jabberClient);

            jabberClient.Connect();
        }

        private void ApplyJabberUserConfiguration(JabberClient jabberClient)
        {
            jabberClient.User = jabberConfig.Username;
            jabberClient.Password = jabberConfig.Password;
            jabberClient.NetworkHost = jabberConfig.Server;
            jabberClient.Server = jabberConfig.ServerName;
        }
    }
}
