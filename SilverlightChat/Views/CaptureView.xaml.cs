using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel.Composition;
using SilverlightChat.Common;
using SilverlightChat.ViewModels;
using System.Windows.Media;

namespace SilverlightChat.Views
{
    /// <summary>
    /// Description for CaptureView.
    /// </summary>
    public partial class CaptureView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the CaptureView class.
        /// </summary>
        public CaptureView()
        {
            InitializeComponent();

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                PartInitializer.SatisfyImports(this);
            }

            //  can't bind the fill of the rectangle to videobrush don't know why
            Messenger.Default.Register<bool>(this, MessageTypes.CameraCapturing, p => { UpdateCameraButton(p); });
            Messenger.Default.Register<bool>(this, MessageTypes.DesktopCapturing, p => { UpdateDesktopButton(p); });
        }

        private void recDesktop_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CaptureSource = recDesktop;
            Messenger.Default.Send<UIElement>(CaptureSource,MessageTypes.UpdateStreamingSource);
        }

        private void recCamera_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CaptureSource = recCamera;
            Messenger.Default.Send<UIElement>(CaptureSource, MessageTypes.UpdateStreamingSource);
        }

        #region Update UI

        void UpdateDesktopButton(bool Capturing)
        {
            if (Capturing)
            {
                btnSendDesktop.Content = "Stop";
                CaptureSource = recDesktop;
            }
            else
            {
                btnSendDesktop.Content = "Desktop";
            }
        }

        void UpdateCameraButton(bool Capturing)
        {
            if (Capturing)
            {
                recCamera.Fill = (this.DataContext as CaptureViewModel).CameraBrush;
                btnConnectWebcam.Content = "Camera Off";
                CaptureSource = recCamera;
            }
            else
            {
                recCamera.Fill = new SolidColorBrush(Colors.Red);
               btnConnectWebcam.Content = "Camera On";
            }
        }

        #endregion

        #region Propertys
        public UIElement CaptureSource
        {
            get { return (UIElement)GetValue(CaptureSourceProperty); }
            set { SetValue(CaptureSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CaptureSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptureSourceProperty =
            DependencyProperty.Register("CaptureSource", typeof(UIElement), typeof(CaptureView), new PropertyMetadata(null));


        [Import(typeof(CaptureViewModel))]
        public CaptureViewModel ViewModel
        {
            set
            {
                DataContext = value;
            }

        }


        #endregion
    }
}