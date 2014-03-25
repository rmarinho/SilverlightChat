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
using System.Text;

namespace SilverlightChat.Common
{
    public class UdpPacketReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public byte[] Data { get; set; }
        public Guid SourceID { get; set; }
        public DataTypes.DataType DataType { get; set; }
        public IPEndPoint Ip { get; set; }


        public UdpPacketReceivedEventArgs( IPEndPoint ip, byte[] data, Guid sourceid, DataTypes.DataType msgtype)
        {
            this.SourceID = sourceid;
            this.Ip = ip;
            this.DataType = msgtype;

            switch (msgtype)
            {
                case DataTypes.DataType.Login:
                    {
                        this.Message = Encoding.UTF8.GetString(data, 40, data.Length - 40).Replace("\0", "");
                        break;
                    }
                case DataTypes.DataType.Text:
                    {
                        this.Message = Encoding.UTF8.GetString(data, 40, data.Length - 40).Replace("\0", "");
                        break;
                    }
                case DataTypes.DataType.Image:
                    {
                        this.Data = new byte[data.Length - 40];
                        Array.Copy(data, 40, this.Data, 0, (data.Length - 40));
                        break;
                    }
                case DataTypes.DataType.Video:
                    {
                        this.Data = new byte[data.Length - 40];
                        Array.Copy(data, 40, this.Data, 0, (data.Length - 40));
                        break;
                    }
                case DataTypes.DataType.Desktop:
                    {
                        this.Data = new byte[data.Length - 40];
                        Array.Copy(data, 40, this.Data, 0, (data.Length - 40));
                        break;
                    }


                default:
                    break;
            }
        }

    }
}
