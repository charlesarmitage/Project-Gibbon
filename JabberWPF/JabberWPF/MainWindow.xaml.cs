using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            this.MessageToSendTextbox.TextChanged += MessageToSendTextboxOnTextChanged;
            this.MessageToSendTextbox.KeyDown += MessageToSendTextboxOnKeyDown;

        }

        private void MessageToSendTextboxOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if(this.presenter != null)
            {
                this.presenter.SendMessageKeyPressed(keyEventArgs.Key);
            }
        }

        private void MessageToSendTextboxOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            if(this.presenter != null)
            {
                this.presenter.UpdateSendMessageText(this.MessageToSendTextbox.Text);
            }
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
