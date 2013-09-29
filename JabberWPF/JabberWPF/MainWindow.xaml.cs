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
        private ChatModel _chatModel;

        public MainWindow()
        {
            InitializeComponent();
            var presenter = (Presenter) this.DataContext;
        }
        
        private void sendMsgTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
               // SendMessage(sendMsgTextbox.Text);
            }
            else if (e.Key == Key.Escape)
            {

            }
        }


    }
}
