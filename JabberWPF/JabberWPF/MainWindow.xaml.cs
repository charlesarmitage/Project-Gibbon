using System.Windows;
using MahApps.Metro.Controls;

namespace JabberWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly Presenter presenter;

        public MainWindow()
        {
            InitializeComponent();
            this.presenter = (Presenter)DataContext;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            this.ChatPanel.Height = this.Height - this.TransmitPanel.Height - 30;
        }

        private void ChatPanelOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            this.Roster.Height = this.ChatPanel.Height - this.StatusPanel.Height;
        }
    }
}
