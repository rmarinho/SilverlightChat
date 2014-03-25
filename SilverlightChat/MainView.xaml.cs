using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using System.ComponentModel.Composition;
using SilverlightChat.ViewModels;
using SilverlightChat.Views;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Media;
using SilverlightChat.Common;
using SilverlightChat.Entities;
using System.Windows.Interop;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System;

namespace SilverlightChat
{
    /// <summary>
    /// Description for MainView.
    /// </summary>
    public partial class MainView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the MainView class.
        /// </summary>
        public MainView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainView_Loaded);
            if (!ViewModelBase.IsInDesignModeStatic)
            {
                PartInitializer.SatisfyImports(this);
            }
            //after we know the message was sent we cleart the textbox
            Messenger.Default.Register<string>(this, MessageTypes.MessageSent, p => { txtMessage.Text = ""; });
        }

        void MainView_Loaded(object sender, RoutedEventArgs e)
        {

            LoginView loginscreen = new LoginView();
            loginscreen.DataContext = this.DataContext;
            loginscreen.Show();

        }

        [Import(typeof(MainViewModel))]
        public MainViewModel ViewModel
        {
            set
            {
                DataContext = value;
            }
        }
    }
}