using GalaSoft.MvvmLight;
using System.ComponentModel.Composition;
using SilverlightChat.Models;
using SilverlightChat.Entities;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using SilverlightChat.Common;
using System.Collections.ObjectModel;
using System;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Interop;
using System.IO;

namespace SilverlightChat.ViewModels
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : ViewModelBase
    {
        MainModel _model;
        //USING MEF 
        //What do we gain doing this? if it was and interface we could be more decouple, and not using the model project reference? 
        //or maybe this is good if the model was in another xap that uses bigger dlls?
        // since mef is good because it only creates the classes when we want, in this case model as to be always created its the base model of the chatroom... 
        //i will try to figure this out better

        [ImportingConstructor]
        public MainViewModel(MainModel model)
        {
            if (this.IsInDesignMode)
            {
            }
            _model = model;
            Messenger.Default.Register<UIElement>(this, MessageTypes.UpdateStreamingSource, p =>
            {
                _element = p;
               
            });
            _messages = new ObservableCollection<ChatMessage>();
            _friends = new ObservableCollection<Person>();

            InitializeCommands();
            RegisterEvents();
        }

        #region Initialize
        private void RegisterEvents()
        {
            _model.LoginComplete += new System.EventHandler(_model_LoginComplete);
            _model.NewMessage += new System.EventHandler<Models.Events.MessageEventArgs>(_model_NewMessage);
            _model.VideoMessage += new EventHandler<Models.Events.VideoMessageEventArgs>(_model_VideoMessage);
            _model.NewPersonArrive += new System.EventHandler<Models.Events.NewPersonEventArgs>(_model_NewPersonArrive);
        }

        private void InitializeCommands()
        {
     
            _SendPicture = new RelayCommand<UIElement>(p => 
            {
                _element = p;
                _model.SendPicture(new WriteableBitmap(_element, null)); 
            });

            _StreamVideo = new RelayCommand<UIElement>(p =>
            {
                _element = p;
                StartStreaming();
            });

            _Login = new RelayCommand<string>(p => 
            { 
                _model.DoLogin(p); 
            });

            _SendMessage = new RelayCommand<string>(p => 
            { 
                _model.SendMessage(p);
            Messenger.Default.Send<string>("clear", MessageTypes.MessageSent);
            });
        } 
        #endregion

        #region Frame Tickers
        void tFrame_Tick(object sender, EventArgs e)
        {
            //why ?

            _streamvideobmp = new WriteableBitmap(_element, null);
            //_streamvideobmp.Render(_element,null);
            _model.SendVideo(_streamvideobmp);

        }

         #endregion
     
        #region Events
 
        //video message as arrive
        void _model_VideoMessage(object sender, Models.Events.VideoMessageEventArgs e)
        {
            //if is our message
            if (e.User.Id == this.User.Id)
            {

            }
            else
            {
                // if is already our friend , not a  good way to get the username
                Person friend = GetFriendAsPerson(e.User.Id);

                if (friend != null)
                {
                    friend.Video = e.Result;
                    RaisePropertyChanged("Friends");
                }
                else
                {

                }
            }

        }

        // a new person arrived to the chat room , lets do lots of things... update de chat, send a notify, lets see if he is already on our list of friends
        void _model_NewPersonArrive(object sender, Models.Events.NewPersonEventArgs e)
        {
            if (e.Error == null)
            {

                //it's our new person message
                if (this.User.Id == e.Result.Id)
                {
                  
                }
                else
                {

                    //  _friends.Contains(e.Result) dosent't work don't know why

                    Person friend = GetFriendAsPerson(e.Result.Id);

                    if (friend != null)
                    {
                        //this friends already exists

                    }
                    else
                    {
                      
                        WriteableBitmap tb = new WriteableBitmap(60, 60);
                        tb.Clear(Colors.Orange);


                        e.Result.Video = tb;
                        _friends.Add(e.Result);
                        _messages.Add(new ChatMessage { User = e.Result, Msg = " : Joins the chat", MessageDateTime= DateTime.Now });

                        _model.SendUsername();
                    }

                }
            }
        }
     
        //new text message arrive
        void _model_NewMessage(object sender, Models.Events.MessageEventArgs e)
        {
            //if is our message
            if (e.Result.User.Id == this.User.Id)
                e.Result.User = this.User;
            else
            {
                // if is already our friend , not a  good way to get the username
                Person friend = GetFriendAsPerson(e.Result.User.Id);
              
                if (friend != null)
                {
                    e.Result.User = friend;

                }
                else
                {
  
                    _friends.Add(e.Result.User);
                    
                }
            }
            _messages.Add(e.Result);
        }

        // TODO: must create a event args with a error code to know if logged in
        //TODO: must add authentication
        void _model_LoginComplete(object sender, System.EventArgs e)
        {
            this.User = _model.User;
            _messages.Add(new ChatMessage { Msg = "Welcome to the Silverlight Chat Room", MessageDateTime=DateTime.Now });
            _messages.Add(new ChatMessage { Msg = string.Format("Enjoy {0}", this.User.NickName), MessageDateTime = DateTime.Now });
          
        } 
        #endregion

        #region Commands


        RelayCommand<UIElement> _StreamVideo;
        public RelayCommand<UIElement> StreamVideo
        {
            get { return _StreamVideo; }
        }

        RelayCommand<UIElement> _SendPicture;
            public RelayCommand<UIElement> SendPicture
        {
            get { return _SendPicture; }
        }
        // Login Command
        RelayCommand<string> _Login;
        public RelayCommand<string> Login
        {
            get { return _Login; }
        }

        RelayCommand<string> _SendMessage;
        public RelayCommand<string> SendMessage
        {
            get { return _SendMessage; }
        }

        
        #endregion

        #region Streaming 

        private void StartStreaming()
        {

            if (tFrame == null)
            {
                tFrame = new DispatcherTimer();
                tFrame.Tick += new EventHandler(tFrame_Tick);
                tFrame.Interval = new TimeSpan(0, 0, 0, 0, 500);
                tFrame.Start();
            }
            else
            {
                if (tFrame.IsEnabled)
                {
                    tFrame.Stop();
                }
                else
                {
                    tFrame.Start();
                }
            }
        }

      
        #endregion

        #region Properties

       //uielement a rectangle
        private  UIElement _element;

        WriteableBitmap _streamvideobmp = new WriteableBitmap(300, 200);

        //timer for framerate of streaming the rectangle
        private DispatcherTimer tFrame;

        //capture a image
        private WriteableBitmap _bmp;

        //Current user
        private Person _user;
        public Person User
        {
            get { return _user; }
          
            private set
            {
                if (value != _user)
                {
                    _user = value;
                    RaisePropertyChanged("User");
                }
            }
        }

        ObservableCollection<ChatMessage> _messages;
        public ObservableCollection<ChatMessage> Messages
        {
            get { return _messages; }
            private set
            {
                if (value != _messages)
                {
                    _messages = value;
                    RaisePropertyChanged("Messages");
                }
            }
        }

        ObservableCollection<Person> _friends;
        public ObservableCollection<Person> Friends
        {
            get { return _friends; }
            private set
            {
                if (value != _friends)
                {
                    _friends = value;
                    RaisePropertyChanged("Friends");
                }
            }
        }

        #endregion

        #region  Little Helpers

        //dirty way to get the person maybe i should have a dic<guid,username>
        private Person GetFriendAsPerson(Guid id)
        {
            Person p = null;
            foreach (Person friend in _friends)
            {
                if (friend.Id == id)
                {
                    p = friend;
                   
                }
            }

            return p;
        }

        #endregion

        ~MainViewModel()
        {
           

        }
    }
}
