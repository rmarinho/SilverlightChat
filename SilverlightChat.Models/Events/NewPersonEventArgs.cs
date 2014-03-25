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
using SilverlightChat.Entities;

namespace SilverlightChat.Models.Events
{

    public class NewPersonEventArgs : EventArgs
    {
        public NewPersonEventArgs(Exception ex)
        {
            _error = ex;
        }
        public NewPersonEventArgs(Person newperson)
        {
            if (newperson == null)
            {
                _error = new Exception("No person");
            }
         
            else
            {
                _result = newperson;
                _error = null;
            }
        }

        // a mesma aplicaçao a correr tem o mesmo gui, tenho k mandar o gui para saber os amigos 

        private Person _result;

        public Person Result
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