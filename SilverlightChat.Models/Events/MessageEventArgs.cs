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
using System.Collections.Generic;
using SilverlightChat.Entities;

namespace SilverlightChat.Models.Events
{

    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(Exception ex)
        {
            _error = ex;
        }
        public MessageEventArgs(ChatMessage msg)
        {
            if (msg == null)
            {
                _error = new Exception("No results");
            }
            if (msg == null)
            {
                _error = new Exception("No sender");
            }
            else
            {
                _result = msg;
                _error = null;
            }
        }

      // a mesma aplicaçao a correr tem o mesmo gui, tenho k mandar o gui para saber os amigos 
        
        private ChatMessage _result;

        public ChatMessage Result
        {
            get { return _result; }
        }

        private Exception _error;

        public Exception Error
        {
            get { return _error; }
            set { _error = value; }
        }
    }
}
