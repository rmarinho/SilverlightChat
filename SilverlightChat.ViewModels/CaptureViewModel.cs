using GalaSoft.MvvmLight;
using System.ComponentModel.Composition;
using GalaSoft.MvvmLight.Command;
using SilverlightChat.Common;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Interop;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using SilverlightChat.Models;

namespace SilverlightChat.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    ///  

    [Export(typeof(CaptureViewModel))]
    public class CaptureViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the CaptureViewModel class.
        /// </summary>
        public CaptureViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real": Connect to service, etc...
            ////}
            _RequestCameraAcess = new RelayCommand(() => { State = "WebCamState"; RequestVideoDevice(); });
            _StartDesktop = new RelayCommand(() => { State = "DesktopState"; CaptureDesktop(); }, () => Application.Current.HasElevatedPermissions);
            Application.Current.Exit += new EventHandler(Current_Exit);
            Application.Current.InstallStateChanged += new EventHandler(Current_InstallStateChanged);
        }

        #region Events
        void Current_InstallStateChanged(object sender, EventArgs e)
        {
            _StartDesktop.RaiseCanExecuteChanged();
        }

        void Current_Exit(object sender, EventArgs e)
        {
            base.Cleanup();
        }
        
        #endregion
     
        #region Commands

        RelayCommand _StartDesktop;
        public RelayCommand StartDesktop
        {
            get { return _StartDesktop; }
        }
        RelayCommand _RequestCameraAcess;
        public RelayCommand AccessCamera
        {
            get { return _RequestCameraAcess; }
        } 
        #endregion
            
        #region Properties

        private bool flagUpdateDesktop = true;

        BitmapImage bitpesk = new BitmapImage();

        //timer for framerate of desktop
        private DispatcherTimer tFrameDesktop;

        //camera device source
        private CaptureSource _capture;

        dynamic desktopComHelper;

        VideoBrush _cameraBrush;
        public VideoBrush CameraBrush
        {
            get { return _cameraBrush; }

            private set
            {
                if (value != _cameraBrush)
                {
                    _cameraBrush = value;
                    RaisePropertyChanged("CameraBrush");
                }
            }
        }
      
        //the brush to paint the rectangle with our live desktop feed
        ImageBrush _desktopBrush;
        public ImageBrush DesktopBrush
        {
            get { return _desktopBrush; }

            private set
            {
                if (value != _desktopBrush)
                {
                    _desktopBrush = value;
                    RaisePropertyChanged("DesktopBrush");
                }
            }

        }

        string _state;
        public string  State
        {
            get { return _state; }

            private set
            {
                if (value != _state)
                {
                    _state = value;
                    RaisePropertyChanged("State");
                }
            }

        }

         #endregion

        #region Private Methods
        private void RequestVideoDevice()
        {

             if (CaptureDeviceConfiguration.RequestDeviceAccess())
            {
                if (_capture == null)
                {

                    VideoCaptureDevice webcam = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();
                    _capture = new CaptureSource();
                    _capture.VideoCaptureDevice = webcam;

                    if (_cameraBrush == null)
                    {
                        _cameraBrush = new VideoBrush();

                    }

                    _cameraBrush.SetSource(_capture);
                    _cameraBrush.Stretch = Stretch.UniformToFill;

                    _capture.Start();
                    Messenger.Default.Send<bool>(true, MessageTypes.CameraCapturing);

                }
                else
                {
                    if (_capture.State == CaptureState.Started)
                    {
                        _capture.Stop();
                        Messenger.Default.Send<bool>(false, MessageTypes.CameraCapturing);
                        State = "DefaultState";
                    }
                    else
                    {
                        Messenger.Default.Send<bool>(true, MessageTypes.CameraCapturing);
                        _capture.Start();

                    }
                }
            }
        }
        private void CaptureDesktop()
        {
            if (tFrameDesktop == null)
            {
                if (Application.Current.HasElevatedPermissions)
                {
                    tFrameDesktop = new DispatcherTimer();
                    tFrameDesktop.Tick += new EventHandler(tFrameDesktop_Tick);
                    tFrameDesktop.Interval = new TimeSpan(0, 0, 0, 0, 100);
                    tFrameDesktop.Start();
                    DesktopBrush = new ImageBrush();

                    desktopComHelper = ComAutomationFactory.CreateObject("CaptureHelpers.COM");
                    if (desktopComHelper == null)
                    {
                        MessageBox.Show("Dll CaptureHelpers not found in your computer");
                    }

                }
                else
                {
                    MessageBox.Show("You need to run with elevated permissions");
                }
            }
            else
            {
                if (tFrameDesktop.IsEnabled)
                {
                    tFrameDesktop.Stop();
                    //update UI
                    Messenger.Default.Send<bool>(false, MessageTypes.DesktopCapturing);
                    flagUpdateDesktop = true;
                    State = "DefaultState";
                }
                else
                {
                    tFrameDesktop.Start();
                }
            }


        }
        void tFrameDesktop_Tick(object sender, EventArgs e)
        {
            //am i creating new array of bytes and not desposing? 
            MemoryStream ms = new MemoryStream();

            byte[] buffer = desktopComHelper.NewImageDesktop();
            ms.Write(buffer, 0, buffer.Length);
            bitpesk.SetSource(ms);
            _desktopBrush.ImageSource = bitpesk;
            if (flagUpdateDesktop)
            {
                Messenger.Default.Send<bool>(true, MessageTypes.DesktopCapturing);
                flagUpdateDesktop = false;
            }
            ms.Dispose();


        } 
        #endregion

       public override void Cleanup()
        {
            desktopComHelper = null;
            // Clean own resources if needed
            if (_capture != null)
            {
                _capture.Stop();
                _capture = null;
                this._cameraBrush = null;

            }

            if (tFrameDesktop != null)
            {
                tFrameDesktop.Stop();
            }
            
            base.Cleanup();
        }
    }
}