using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;







/*******************************************************/
/*
项目: HFCQMES
模块: 
描述: Socket通讯
版本: 1.0
日期: 2015.09.12
作者: 刘晓春

更新:
TODO: */
/*******************************************************/

namespace SocketClienTool.Code.SocketHelper
{
    public class SocketServer
    {

        public event EventHandler<string> OnRecData; //定义一个委托类型的事件  
        public event EventHandler<Tuple<string, string>> OnClientConnected; // 客户端接入事件<IP, Port>
        public event EventHandler<string> OnCleintClose; //SocketClient 关闭  

        private Boolean canlistening = false;
        private int Port = 10000;
        Dictionary<string, Socket> socketLst = new Dictionary<string, Socket>();


        public SocketServer(int port)
        {
            this.Port = port;

        }

        public void Start()
        {
            Thread LisThread = new Thread(new ThreadStart(StartListening));
            LisThread.Start();


        }
        public void Stop()
        {
            canlistening = false;
            allDone.Set();
            foreach (KeyValuePair<string, Socket> pair in socketLst)
            {
                Socket socket = pair.Value;
                if (socket != null)
                {
                    socket.Close();
                }
            }

            socketLst.Clear();

        }

        private ManualResetEvent allDone = new ManualResetEvent(false);

        public void StartListening()
        {
            canlistening = true;
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);
            Socket listener = null;

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(110);
                //LogUnit.Debug("服务监听端口" + Port);

                while (canlistening)
                {
                    allDone.Reset();
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    allDone.WaitOne();            //阻塞主线程 
                }

            }
            catch (Exception ex)
            {

                throw new Exception("listener.Bind=" + ex.ToString());

            }
            finally
            {
                listener.Close();
              //  LogUnit.Debug("finally 停止接收");

            }

        }



        public void AcceptCallback(IAsyncResult ar)
        {
            if (canlistening)
            {

                Socket listener = (Socket)ar.AsyncState;

                Socket handler = listener.EndAccept(ar);
                IPEndPoint endPoint = ((IPEndPoint)handler.RemoteEndPoint);
                String clientip = endPoint.Address.ToString();
                String clientport = endPoint.Port.ToString();
                //设置主线程继续

                allDone.Set();

                StateObject state = new StateObject();

                state.workSocket = handler;

                // 2019-11-14 修改 保证1个IP 只有一个连接
                AddtoCliet(clientip, handler);
                //string key = clientip + ":" + clientport;
                //socketLst.Add(key, handler);
                //OnRecData(handler,"客户端：" +key +" 链接");
                OnClientConnected?.Invoke(handler, new Tuple<string, string>(clientip, clientport));
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,

                    new AsyncCallback(ReadCallback), state);
            }

        }

        /// <summary>
        /// 2019-11-14 修改 保证1个IP 只有一个连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="stocket"></param>
        public void AddtoCliet(string ip,Socket stocket) {
            if (socketLst.ContainsKey(ip))
            {
                Socket oldsocket = socketLst[ip];
                oldsocket.Close();
                oldsocket = null;
                socketLst[ip] = stocket;
            }
            else {
                socketLst.Add(ip, stocket);
            }
           
        }

        public void ReadCallback(IAsyncResult ar)
        {
            try
            {
                if (!canlistening) return;


                StateObject state = (StateObject)ar.AsyncState;

                Socket handler = state.workSocket;

                IPEndPoint clientip = (IPEndPoint)handler.RemoteEndPoint;
                String ip = clientip.Address.ToString();
                int read = handler.EndReceive(ar);
                if (canlistening & read > 0)
                {
                    //处理接收到的命令
                    //  StocketDataWinderPLCHandler micHandler = new StocketDataWinderPLCHandler(state.buffer, 0, read, doffer);
                    string readDataString = Encoding.UTF8.GetString(state.buffer, 0, read);
                 //   LogUnit.Debug("rec client ip:" + ip + " readDataString=" + readDataString);
                    //  byte[] newfuf = ByteHelper.SubArr(state.buffer, 0, read);

                    OnRecData(handler, readDataString);

                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);

                }

                else
                {

                    if (state.sb.Length > 1)
                    {

                        string content = state.sb.ToString();
                      //  FileUnit.Log(string.Format("Read {0} bytes from socket.\n Data:{1}", content.Length, content));

                    }

                    handler.Close();
                    handler = null;


                }
            }
            catch (Exception ex)
            {
               // LogUnit.Error(this.GetType(), ex);
            }


        }
        public Socket GetSocketByIp(string clientip)
        {
            foreach (KeyValuePair<string, Socket> item in socketLst)
            {
                if (item.Key.StartsWith(clientip))
                {
                    return item.Value;
                }
            }
            return null;
        }
        //    public Socket GetSocketByIp(string clientip)
        //{
        //    List<string> keys = new List<string>(socketLst.Keys);
        //    int icount = keys.Count;
        //    for (int i = icount-1; i >=0; i--)
        //    {

        //        Socket socket = socketLst[keys[i]];
        //        if (socket != null)
        //        {
        //            if (socket.Connected == false)
        //            {
        //                socket = null;
        //                socketLst.Remove(keys[i]);

        //            }
        //            else {
        //                if (keys[i].StartsWith(clientip)) {
        //                    return socket;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            socketLst.Remove(keys[i]);

        //        }

        //    }

        //    return null;

        //}
        public bool SendToClient(string ip, string msg)
        {
            try
            {
                Socket socket = GetSocketByIp(ip);
                if (socket != null)
                {
                    socket.Send(Encoding.UTF8.GetBytes(msg));
                }
                else
                {
                   // SocketNotSendManager.Add(ip, msg);

                }
                return false;
            }
            catch (Exception ex) {
               // LogUnit.Error(this.GetType(), ex);
                return false;

            }
          

        }


        public void ShowClient()
        {
            //FileUnit.Log("Count:" + socketLst.Count);

            foreach (KeyValuePair<string, Socket> pair in socketLst)
            {

                Socket socket = pair.Value;
                if (socket != null)
                {
                   // FileUnit.Log(pair.Key + " " + socket.Connected.ToString());
                }
                else
                {
                    socketLst.Remove(pair.Key);
                }

            }

        }
      
        public void SendHeartBeat()
        {

            List<string> keys = new List<string>(socketLst.Keys);
            int icount = keys.Count;
            for (int i = icount - 1; i >= 0; i--)
            {
                Socket socket = socketLst[keys[i]];
                if (socket != null)
                {
                    if (socket.Connected)
                    {
                     //  SendHeartBeat(socket);
                    }
                    else
                    {
                        OnCleintClose(this, keys[i]);
                        socketLst.Remove(keys[i]);
                    }

                }

            }
        }

    }
}