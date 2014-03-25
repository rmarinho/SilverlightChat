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
using System.ComponentModel;

namespace SilverlightChat.Entities
{

    public class Person : INotifyPropertyChanged 
    {
        /// <summary>
        /// The unique ID 
        /// </summary>
        public Guid  Id { get; set; }

        /// <summary>
        /// The nickname of the person
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// The of the endpoint
        /// </summary>
        public IPEndPoint IP { get; set; }

        /// <summary>
        /// The stream of webcam 
        /// </summary>
        /// 


        ImageSource _video;
        public ImageSource  Video
        {
            get
            {
                return _video;
            }
            set
            {
                 _video = value;
                onPropertyChanged(this, "Video"); 
            }
        }

        
         /// <summary>
        /// Avatar image
        /// </summary>
        public Image Avatar { get; set; }


        private void onPropertyChanged(object sender, string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
