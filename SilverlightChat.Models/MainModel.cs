using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using FluxJpeg.Core;
using FluxJpeg.Core.Encoder;
using SilverlightChat.Common;
using SilverlightChat.Entities;
using SilverlightChat.Models.Events;

namespace SilverlightChat.Models
{
    [Export(typeof(MainModel))]
    public class MainModel 
    {
        //this should be a interface 
        public event EventHandler LoginComplete;
        public event EventHandler LogoutComplete;
        public event EventHandler<MessageEventArgs> NewMessage;
        public event EventHandler<NewPersonEventArgs> NewPersonArrive;
        public event EventHandler<VideoMessageEventArgs> VideoMessage; 
       
        #region Public Methods

        //login incialize and open 
        public void DoLogin(string username)
        {  
            if (_user == null)
            {
                _user = new Person { NickName = username, Id= Guid.NewGuid() };
                
            }

           this.Channel.OpenCompleted += new EventHandler(Channel_OpenCompleted);
           this.Channel.CloseCompleted += new EventHandler(Channel_CloseCompleted);
           this.Channel.PacketReceived += new EventHandler<UdpPacketReceivedEventArgs>(Channel_PacketReceived);
           this.Channel.Open();
           

        }

        //send new chat message
        public void SendMessage(string newmsg)
        {
            _channel.Send(this.User.Id, newmsg);
        }
      
        //send our username when we arriver or when someone arrives, i tryed the sendto the ip but it didin't work, so we are reving our own content
        public void SendUsername()
        {
            this.Channel.SendUsername(_user.Id, _user.NickName);
        }

        //send picture as a message
        public void SendPicture(WriteableBitmap bmp)
        {
            MemoryStream bufferstream = new MemoryStream();
            EncodeJpeg(bmp, bufferstream);

            _channel.SendImage(this.User.Id, bufferstream.ToArray());

            bufferstream.Dispose();
            bufferstream = null;
           
        }

        //send video in a byte array
        public void SendVideo(WriteableBitmap bmpVideo)
        {
            //should encode to jpeg here
            MemoryStream bufferstream = new MemoryStream();
            EncodeJpeg(bmpVideo, bufferstream);
            
            _channel.SendVideo(this.User.Id, bufferstream.ToArray());
           
            bufferstream.Dispose();
            bufferstream = null;
        }

        //send a stream of desktop 
        public void SendDesktopVideo(byte[] DesktopVideoBuffer)
        {
            //this would be use to send the two sources
            _channel.SendDesktop(this.User.Id, DesktopVideoBuffer);
        }

        #endregion

        #region Channel Events

        void Channel_PacketReceived(object sender, UdpPacketReceivedEventArgs e)
        {

            switch (e.DataType)
            {
                case DataTypes.DataType.Login:
                    EventHandler<NewPersonEventArgs> handlerNewPerson = this.NewPersonArrive;
                    if (handlerNewPerson != null)
                        handlerNewPerson(this, new NewPersonEventArgs(new Person { NickName = e.Message, IP = e.Ip, Id = e.SourceID }));
                    break;

                case DataTypes.DataType.Text:
                    {
                        EventHandler<MessageEventArgs> handler = this.NewMessage;
                        if (handler != null)
                            handler(this, new MessageEventArgs(new ChatMessage { Msg = e.Message, User = new Person { Id = e.SourceID, IP = e.Ip }, MessageDateTime = DateTime.Now }));
                        break;
                    }
                case DataTypes.DataType.Image:
                    {

                        MemoryStream ms = new MemoryStream();
                        ms.Write(e.Data, 0, e.Data.Length);
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(ms);
                        EventHandler<MessageEventArgs> handlerNewPictureMessage = this.NewMessage;
                        if (handlerNewPictureMessage != null)
                            handlerNewPictureMessage(this, new MessageEventArgs(new ChatMessage { Msg = "Look at this picture", User = new Person { Id = e.SourceID, IP = e.Ip }, Image = bitmapImage, MessageDateTime = DateTime.Now }));
                       
                        bitmapImage = null;
                        ms.Dispose();
                        ms = null;
                        break;
                    }
                case DataTypes.DataType.Video:
                    {
                        MemoryStream ms = new MemoryStream();
                        ms.Write(e.Data, 0, e.Data.Length);
                        BitmapImage bitmapVideo = new BitmapImage();
                        bitmapVideo.SetSource(ms);
                        EventHandler<VideoMessageEventArgs> handlerVideoeMessage = this.VideoMessage;
                        if (handlerVideoeMessage != null)
                        {
                            handlerVideoeMessage(this, new VideoMessageEventArgs(bitmapVideo, new Person { Id = e.SourceID, IP = e.Ip }));
                        }
                        bitmapVideo = null;
                        ms.Dispose();
                        ms = null;
                        break;
                    }
                case DataTypes.DataType.Desktop:
                    {
                        EventHandler<VideoMessageEventArgs> handlerVideoeMessage = this.VideoMessage;
                        MemoryStream ms = new MemoryStream();
                        ms.Write(e.Data, 0, e.Data.Length);
                        BitmapImage bitpesk = new BitmapImage();
                        bitpesk.SetSource(ms);
                        if (handlerVideoeMessage != null)
                        {
                            handlerVideoeMessage(this, new VideoMessageEventArgs(bitpesk, new Person { Id = e.SourceID, IP = e.Ip }));
                        }
                        break;
                    }
                default:



                    break;
            }
        }

        void Channel_CloseCompleted(object sender, EventArgs e)
        {
            //   this.Channel.Send(this.User.Id, "{0}: leaves the chat room", _user.NickName);
            this.Channel.CloseCompleted -= new EventHandler(Channel_CloseCompleted);
        }

        void Channel_OpenCompleted(object sender, EventArgs e)
        {
            //raise login completed event or use messagging?

            EventHandler handler = this.LoginComplete;

            if (handler != null)
                handler(this, EventArgs.Empty);

            this.Channel.OpenCompleted -= new EventHandler(Channel_OpenCompleted);

            //  this.Channel.Send(_user.EndPointId, "{0}: join the chat room", _user.NickName);
            //we can also send a request for all clients: give me your usernames or maybe if we receive a new login, we just respond to that sending our own, for that user only
            this.SendUsername();

        } 
        #endregion
     
        #region Jpeg helpers

        //http://kodierer.blogspot.com/2009/11/convert-encode-and-decode-silverlight.html

        public static void EncodeJpeg(WriteableBitmap bmp, Stream destinationStream)
        {
            // Init buffer in FluxJpeg format
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int[] p = bmp.Pixels;
            byte[][,] pixelsForJpeg = new byte[3][,]; // RGB colors
            pixelsForJpeg[0] = new byte[w, h];
            pixelsForJpeg[1] = new byte[w, h];
            pixelsForJpeg[2] = new byte[w, h];

            // Copy WriteableBitmap data into buffer for FluxJpeg
            int i = 0;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int color = p[i++];
                    pixelsForJpeg[0][x, y] = (byte)(color >> 16); // R
                    pixelsForJpeg[1][x, y] = (byte)(color >> 8);  // G
                    pixelsForJpeg[2][x, y] = (byte)(color);       // B
                }
            }

            // Encode Image as JPEG using the FluxJpeg library
            // and write to destination stream
            ColorModel cm = new ColorModel { colorspace = ColorSpace.RGB };
            FluxJpeg.Core.Image jpegImage = new FluxJpeg.Core.Image(cm, pixelsForJpeg);
            JpegEncoder encoder = new JpegEncoder(jpegImage, 95, destinationStream);
            encoder.Encode();
        }

        public static WriteableBitmap DecodeJpeg(Stream sourceStream)
        {
            // Decode JPEG from stream
            var decoder = new FluxJpeg.Core.Decoder.JpegDecoder(sourceStream);
            var jpegDecoded = decoder.Decode();
            var img = jpegDecoded.Image;
            img.ChangeColorSpace(ColorSpace.RGB);

            // Init Buffer
            int w = img.Width;
            int h = img.Height;
            var result = new WriteableBitmap(w, h);
            int[] p = result.Pixels;
            byte[][,] pixelsFromJpeg = img.Raster;

            // Copy FluxJpeg buffer into WriteableBitmap
            int i = 0;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    p[i++] = (0xFF << 24)                    // A
                           | (pixelsFromJpeg[0][x, y] << 16) // R
                           | (pixelsFromJpeg[1][x, y] << 8)  // G
                           | pixelsFromJpeg[2][x, y];       // B
                }
            }

            return result;
        } 
        #endregion

        #region Properties

        //think that this is very important shoud be here, or not...  
        private Person _user { get; set; }

        public Person User { get { return _user; } }

        private UdpAnySourceMulticastChannel _channel;

        public UdpAnySourceMulticastChannel Channel
        {
            get
            {
                if (_channel == null)
                {
                    _channel = new UdpAnySourceMulticastChannel(
                        //IPAddress.Parse("224.0.0.1"), 3000);
                        IPAddress.Parse("239.0.0.5"), 45678);
                }

                return _channel;
            }
        } 
        #endregion

        ~MainModel()
        {
            //in destructur free resources
            
        }
    }
}
