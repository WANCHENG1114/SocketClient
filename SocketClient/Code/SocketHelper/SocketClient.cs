



using SocketClienTool.Code.ConvertEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketClienTool.Code.SocketHelper
{

    public class SocketClient
    {
        // public event EventHandler<string> OnRecData; //定义一个委托类型的事件  
        public event EventHandler<byte[]> OnRecData; //定义一个委托类型的事件  
        public event EventHandler<string> OnLog; //定义一个委托类型的事件  
        private Boolean Listening = false;
        private Socket ClientSocket = null;
      //  private bool Doing = false; //处理数据
        string host;
        int port;
        private Dictionary<string, string> PrintList = new Dictionary<string, string>();

        private bool SocketconState { get; set; }

        public SocketClient(string _host, int _port)
        {
            host = _host;
            port = _port;
        }
        /// <summary>
        /// 链接服务
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            Stop();
            return ConnectServer();

        }

        private ManualResetEvent allDone = new ManualResetEvent(false);
        #region Socket
        //   byte[] MsgBuffer = new byte[1024];
        /// <summary>
        /// 打开客户端，即连接服务器
        /// </summary>
        private bool ConnectServer()
        {
            try
            {
                Listening = true;


                //创建终结点EndPoint
                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint remotePoint = new IPEndPoint(ip, port);   //把ip和端口转化为IPEndPoint的实例

                ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //ClientSocket.ReceiveTimeout
                ClientSocket.Connect(remotePoint);
                //发送信息至服务器
                // ClientSocket.Send(Encoding.Unicode.GetBytes("用户： 进入系统！" + "\r\n"));


                StateObject state = new StateObject();
                state.workSocket = ClientSocket;

                ClientSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None,
                    new AsyncCallback(ReceiveCallback), state);


                SocketconState = true;
                Log("连接到服务器IP：" + host);
                return true;


            }
            catch (ArgumentException e)
            {
              //  LogUnit.Error("argumentNullException:" + e.Message);
                Stop();
                return false;
            }
            catch (SocketException e)
            {
              //  LogUnit.Error("SocketException:" + e.Message);
                Stop();
                return false;
            }
            catch (Exception e)
            {
              //  LogUnit.Error("SocketException:" + e.Message);
                Stop();
                return false;
            }


        }

        /// <summary>
        /// 回调时调用
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            SocketconState = true;
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;
                int read = handler.EndReceive(ar);
                //  LogUnit.Debug("ReceiveCallback read=" + read);
                // LogUnit.Debug("ReceiveCallback state.buffer.Length=" + state.buffer.Length);
                
                if (Listening && read > 0)
                {
                    byte[] newfuf = ByteHelper.SubArr(state.buffer, 0, read);//截取需要的字节byte
                    OnRecData(this, newfuf);

                    ClientSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    this.Stop();

                }
            }
            catch (Exception ex)
            {
                this.Stop();
            }
        }

        /// <summary>
        /// 发送数据到服务器
        /// </summary>
        /// <param name="sendStr"></param>
        /// <returns></returns>
        public bool SendToService(string sendStr)
        {
            try
            {
                if (ClientSocket == null || ClientSocket.Connected == false)
                {
                    Start();
                    if (ClientSocket.Connected == false)
                    {
                        return false;
                    }
                }
                //向服务器发送信息
                if (sendStr.Trim() != "HeartBeat") // 过滤心跳
                {
                   // LogUnit.Debug("Send:" + sendStr);
                }

                byte[] bs = Encoding.UTF8.GetBytes(sendStr);   //把字符串编码为字节
                // bs = Protocol.Packet(bs);
                int x = ClientSocket.Send(bs, bs.Length, 0); //发送信息
                SocketconState = true;
                return true;
            }
            catch (Exception ex)
            {
                //LogUnit.Error(this.GetType(), ex);
                throw new Exception("发送失败 " + ex.Message);
            }

        }
        /// <summary>
        /// 发送数据到服务器
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public bool SendToService(byte[] bs)
        {
            try
            {
#if DEBUG
                //string hexstr = ByteHelper.ByteArrayToHexString(bs);
                //string function = hexstr.Substring(33, 2); // 取功能号
               // if (function != "00") FileUnit.Log("【DEBUG】Send to PLC Hex = " + hexstr); // 心跳包不记录日志
#endif
                if (ClientSocket == null || ClientSocket.Connected == false)
                {
                    Start();
                    if (ClientSocket.Connected == false)
                    {
                        return false;
                    }
                }
                int x = ClientSocket.Send(bs, bs.Length, 0); //发送信息
                                                             //LogUnit.Debug("x=" + x);
                SocketconState = true;
                return true;
            }
            catch (Exception ex)
            {
                //LogUnit.Error(this.GetType(), ex);
                throw new Exception("发送失败：" + ex.Message);
            }
        }
        #endregion
        /// <summary>
        /// 关闭链接
        /// </summary>
        public void Stop()
        {
            Listening = false;
            SocketconState = false;
            if (ClientSocket != null)
            {
                ClientSocket.Close();
                ClientSocket = null;
            }
        }
        /// <summary>
        /// Socket 连接状态
        /// </summary>
        /// <returns></returns>
        public bool ConnetionState()
        {
            if (SocketconState)
            {
                if (ClientSocket == null || ClientSocket.Connected == false)
                {
                    SocketconState = false;
                }
                return SocketconState;
            }
            else
            {
                return ConnectServer();
            }
        }

        public void Log(string msg) {
            if (OnLog != null) {
                OnLog(this, msg);
            } 
        }

    }
}