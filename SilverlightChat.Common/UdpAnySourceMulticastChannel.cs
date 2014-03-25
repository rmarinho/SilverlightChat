using System;
using System.Net;
using System.Windows;
using System.Net.Sockets;
using System.Text;

namespace SilverlightChat.Common
{
    public class UdpAnySourceMulticastChannel : IDisposable
    {
        //Buffer data from transmittion
        private byte[] ReceiveBuffer { get; set; }

        private byte[] SendingBuffer { get; set; }

        //send the package event to clients
        public event EventHandler<UdpPacketReceivedEventArgs> PacketReceived;

        private UdpAnySourceMulticastClient Client { get; set; }

        //max buffer size ( if i send a image 300*300 = array of 1280000 silverlight crash)
        //TODO: this shoule by a configuarition setting
        public const Int32 buffersize = 50000;

        //Do we already disposed?
        public bool IsDisposed { get; private set; }

        //Do we already joined the party?
        public bool IsJoined { get; private set; }


        public UdpAnySourceMulticastChannel(IPAddress address, int port)
        {
            //inicialize our buffer array
            this.ReceiveBuffer = new byte[buffersize];
            this.SendingBuffer = new byte[buffersize];

            //inicialize our MulticastClient
            //should this be in other class??
            this.Client = new UdpAnySourceMulticastClient(address, port);

        }

        //release resources
        //clear the array?
        public void Dispose()
        {
            if (!IsDisposed)
            {
                this.IsDisposed = true;

                this.ReceiveBuffer = null;
                this.SendingBuffer = null;

                if (this.Client != null)
                    this.Client.Dispose();
            }
        }

        public void Open()
        {
            if (!this.IsJoined)
            {
                this.Client.BeginJoinGroup(
                    result =>
                    {
                        this.Client.EndJoinGroup(result);
                        this.IsJoined = true;
                        Deployment.Current.Dispatcher.BeginInvoke(
                            () =>
                            {
                                this.Receive();
                                this.OnAfterOpen();

                            });
                    }, null);
            }
        }


        private void OnAfterOpen()
        {
            //we can only set buffersize on the client after open
            this.Client.ReceiveBufferSize = buffersize;
            this.Client.SendBufferSize = buffersize;
            FireEvent(OpenCompleted);
        }

        //after we open lets send a event so the model updates
        //TODO: can we send more good data here in the events? and create custom eventargs for it?
        public event EventHandler OpenCompleted;
        public event EventHandler CloseCompleted;


        public void Close()
        {
            this.IsJoined = false;
            this.Dispose();
        }

        private void OnBeforeClose()
        {
            FireEvent(CloseCompleted);
        }

        //Let's receive data, if we are in the channel lets clear our buffer and receive packets
        private void Receive()
        {
            if (this.IsJoined)
            {
                Array.Clear(this.ReceiveBuffer, 0, this.ReceiveBuffer.Length);

                this.Client.BeginReceiveFromGroup(this.ReceiveBuffer, 0, this.ReceiveBuffer.Length,
                   result =>
                   {

                       IPEndPoint source;
                       this.Client.EndReceiveFromGroup(result, out source);

                       Deployment.Current.Dispatcher.BeginInvoke(
                               () =>
                               {
                                   this.OnReceive(source, this.ReceiveBuffer);
                                   this.Receive();
                               });


                   }, null);
            }
        }

        //we are in the chat room, lets send a memssage with our nick name to all of 
        public void SendUsername(Guid sourceid, string Username)
        {
            if (this.Client == null)
                return;

            if (this.IsJoined)
            {
                Array.Clear(this.SendingBuffer, 0, this.SendingBuffer.Length);
                //insert the datatype 4 bytes
                Array.Copy(Encoding.UTF8.GetBytes(DataTypes.Login), SendingBuffer, 4);
                //insert the guid of the client 36 bytes (one per app is better then use ip )
                Array.Copy(Encoding.UTF8.GetBytes(sourceid.ToString()), 0, SendingBuffer, 4, 36);
                //insert the username
                Array.Copy(Encoding.UTF8.GetBytes(Username), 0, SendingBuffer, 40, Encoding.UTF8.GetBytes(Username).Length);

                this.Client.BeginSendToGroup(SendingBuffer, 0, SendingBuffer.Length,
                 result =>
                 {
                     this.Client.EndSendToGroup(result);
                 }, sourceid);
            }
        }

        public void Send(Guid sourceid, string format, params object[] args)
        {
            if (this.Client == null)
                return;

            if (this.IsJoined)
            {
                //TODO: this should be done outside , here should't have this type of logic

                //clear the array
                Array.Clear(this.SendingBuffer, 0, this.SendingBuffer.Length);
                //insert the datatype 4 bytes
                Array.Copy(Encoding.UTF8.GetBytes(DataTypes.Text), SendingBuffer, 4);
                //insert the guid of the client 36 bytes (one per app is better then use ip )
                Array.Copy(Encoding.UTF8.GetBytes(sourceid.ToString()), 0, SendingBuffer, 4, 36);
                //copy the rest of the array
                Array.Copy(Encoding.UTF8.GetBytes(string.Format(format, args)), 0, SendingBuffer, 40, Encoding.UTF8.GetBytes(string.Format(format, args)).Length);


                this.Client.BeginSendToGroup(SendingBuffer, 0, SendingBuffer.Length,
                    result =>
                    {
                        this.Client.EndSendToGroup(result);
                    }, sourceid);
            }
        }

        public void Send(Guid sourceid, string message)
        {
            if (this.Client == null)
                return;

            if (this.IsJoined)
            {
                //TODO: this should be done outside , here should't have this type of logic

                //clear the array
                Array.Clear(this.SendingBuffer, 0, this.SendingBuffer.Length);
                //insert the datatype 4 bytes
                Array.Copy(Encoding.UTF8.GetBytes(DataTypes.Text), SendingBuffer, 4);
                //insert the guid of the client 36 bytes (one per app is better then use ip )
                Array.Copy(Encoding.UTF8.GetBytes(sourceid.ToString()), 0, SendingBuffer, 4, 36);
                //copy the rest of the array
                Array.Copy(Encoding.UTF8.GetBytes(message), 0, SendingBuffer, 40, Encoding.UTF8.GetBytes(message).Length);

                //maybe clear the rest of the buffer maybe reduce the length of the buffer
                this.Client.BeginSendToGroup(SendingBuffer, 0, SendingBuffer.Length,
                     result =>
                     {
                         this.Client.EndSendToGroup(result);
                     }, sourceid);
            }
        }

        public void SendImage(Guid sourceid, byte[] data)
        {
            if (this.Client == null)
                return;

            if (this.IsJoined)
            {
                //clear the array
                Array.Clear(this.SendingBuffer, 0, this.SendingBuffer.Length);
                //insert the datatype 4 bytes
                Array.Copy(Encoding.UTF8.GetBytes(DataTypes.Image), SendingBuffer, 4);

                //insert the guid of the client 36 bytes (one per app is better then use ip )
                Array.Copy(Encoding.UTF8.GetBytes(sourceid.ToString()), 0, SendingBuffer, 4, 36);
                //copy the rest of the array
                Array.Copy(data, 0, SendingBuffer, 40, data.Length);

                this.Client.BeginSendToGroup(SendingBuffer, 0, SendingBuffer.Length,
              result =>
              {
                  this.Client.EndSendToGroup(result);
              }, null);

            }
        }

        public void SendVideo(Guid sourceid, byte[] data)
        {
            if (this.Client == null)
                return;

            if (this.IsJoined)
            {
                //clear the array
                Array.Clear(this.SendingBuffer, 0, this.SendingBuffer.Length);
                //insert the datatype 4 bytes
                Array.Copy(Encoding.UTF8.GetBytes(DataTypes.Video), SendingBuffer, 4);

                //insert the guid of the client 36 bytes (one per app is better then use ip )
                Array.Copy(Encoding.UTF8.GetBytes(sourceid.ToString()), 0, SendingBuffer, 4, 36);
                //copy the rest of the array
                Array.Copy(data, 0, SendingBuffer, 40, data.Length);

                this.Client.BeginSendToGroup(SendingBuffer, 0, SendingBuffer.Length,
              result =>
              {
                  this.Client.EndSendToGroup(result);
              }, null);

            }
        }

        public void SendDesktop(Guid sourceid, byte[] data)
        {
            if (this.Client == null)
                return;

            if (this.IsJoined)
            {
                //clear the array
                Array.Clear(this.SendingBuffer, 0, this.SendingBuffer.Length);
                //insert the datatype 4 bytes
                Array.Copy(Encoding.UTF8.GetBytes(DataTypes.Desktop), SendingBuffer, 4);

                //insert the guid of the client 36 bytes (one per app is better then use ip )
                Array.Copy(Encoding.UTF8.GetBytes(sourceid.ToString()), 0, SendingBuffer, 4, 36);
                //copy the rest of the array
                Array.Copy(data, 0, SendingBuffer, 40, data.Length);

                this.Client.BeginSendToGroup(SendingBuffer, 0, SendingBuffer.Length,
              result =>
              {
                  this.Client.EndSendToGroup(result);
              }, null);

            }
        }

        //We receive some data let us use the first 4 bytes to know what type of data it is and raise the event data with the apropriate type
        private void OnReceive(IPEndPoint source, byte[] data)
        {
            EventHandler<UdpPacketReceivedEventArgs> handler = this.PacketReceived;


            switch (Encoding.UTF8.GetString(data, 0, 4).ToString())
            {
                case DataTypes.Login:
                    {
                        if (handler != null)
                            handler(this, new UdpPacketReceivedEventArgs(source, data, Guid.Parse(Encoding.UTF8.GetString(data, 4, 36).ToString()), DataTypes.DataType.Login));
                        break;
                    }

                case DataTypes.Text:
                    {
                        if (handler != null)
                            handler(this, new UdpPacketReceivedEventArgs(source, data, Guid.Parse(Encoding.UTF8.GetString(data, 4, 36).ToString()), DataTypes.DataType.Text));
                        break;
                    }
                case DataTypes.Image:
                    {
                        if (handler != null)
                            handler(this, new UdpPacketReceivedEventArgs(source, data, Guid.Parse(Encoding.UTF8.GetString(data, 4, 36).ToString()), DataTypes.DataType.Image));

                        break;

                    }
                case DataTypes.Video:
                    {
                        if (handler != null)
                            handler(this, new UdpPacketReceivedEventArgs(source, data, Guid.Parse(Encoding.UTF8.GetString(data, 4, 36).ToString()), DataTypes.DataType.Video));

                        break;
                    }
                case DataTypes.Desktop:
                    {
                        if (handler != null)
                            handler(this, new UdpPacketReceivedEventArgs(source, data, Guid.Parse(Encoding.UTF8.GetString(data, 4, 36).ToString()), DataTypes.DataType.Desktop));

                        break;

                    }
                default:
                    break;
            }
        }

        #region Little Helpers
        public void FireEvent(EventHandler ev)
        {
            EventHandler handler = ev;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
        #endregion


    }
}