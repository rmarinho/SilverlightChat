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

namespace SilverlightChat.Common
{
    public class DataTypes
    {
        public const string Login = "[00]";
              
        public const string Text = "[01]";

        public const string Image = "[02]";

        public const string Video = "[03]";

        public const string Desktop = "[04]";


        public enum DataType
        {
            Login,
            Text,
            Image,
            Video,
            Desktop
        }   
    }

}
