using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightChat.Entities
{
    public class ChatMessage
    {
        public string Msg { get; set; }

        public Person User { get; set; }

        public ImageSource Image { get; set; }

        public  DateTime MessageDateTime { get; set; }

    }
}
