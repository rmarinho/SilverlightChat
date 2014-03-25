using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightChat.Views
{
    public partial class LoginView : ChildWindow
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtUserName.Text))
                this.DialogResult = true;
        }
    }
}

