using SocketClienTool.Code.ConvertEx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SocketClienTool.FrmClient;

namespace SocketClienTool
{
    public partial class FrmServer : Form
    {
        Socket Socket = null;
        Socket temp = null;
        Thread thread = null;
        public FrmServer()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //把ip和端口转为IPEndpoint实例
            int port = Convert.ToInt32(textBox2.Text);
            IPAddress ip = IPAddress.Parse(comboBox1.Text);
            IPEndPoint ipe = new IPEndPoint(ip, port);
            //创建一个Socket 对象
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(ipe); //绑定IPEndpoint的对象
            Socket.Listen(0);//开始监听

            thread = new Thread(new ThreadStart(Serverlisten));
            thread.IsBackground = true;
            thread.Start();
            Log("开启成功");
        }
        private List<byte []> chaifen(byte[] filebyte)
        {
            #region 分解数组 文件byte+附加byte
            List<byte> by = new List<byte>();
            List<byte> by2 = new List<byte>();
            int tt = filebyte.Length - 1024;
            for (int i = 0; i < filebyte.Length; i++)
            {
                if (i <1024 )
                {
                    by.Add(filebyte[i]);
                }
                if (i>=1024)
                {
                    by2.Add(filebyte[i]);
                }
            }
            //Array.Resize(ref filebyte, tt);//将指定数组重新分配大小
            List<byte []> newbyteList = new List<byte[]>();
            newbyteList.Add(by.ToArray());
            newbyteList.Add(by2.ToArray());
            return newbyteList;
            #endregion
        }
        private void Serverlisten()
        {
            temp = Socket.Accept();
            while (true)
            {
                try
                {
                    string recvStr = "";
                    byte[] recvBytes = new byte[31457280] ;//30MB //得填当前文件大小
                    int bytes;
                    bytes = temp.Receive(recvBytes, recvBytes.Length, 0);
                    recvStr += Encoding.GetEncoding("GB2312").GetString(recvBytes, 0, bytes);
                    if (recvStr.EndsWith("0#"))
                    {
                        continue;
                    }

                    List<byte[]> filebytes = chaifen(recvBytes);//处理收到的byte
                    
                    string tt = Encoding.GetEncoding("GB2312").GetString(filebytes[0], 0, 1024);
                    string [] fujia = tt.Split(new char[] { ','});//分解文件大小以及文件类型
                    if (fujia[0] == "###")//文件标志，进行文件处理
                    {
                        Log("客户端" + temp.RemoteEndPoint.ToString() + "：" + fujia[2].Trim("\0".ToCharArray()) + "文件，文件大小：" + int.Parse(fujia[1]));

                        byte[] by = filebytes[1];
                        Array.Resize(ref by, int.Parse(fujia[1]));
                        string str = @"D:\Wcheng\DemoTest\123." + fujia[2].Trim("\0".ToCharArray());
                        if (by.Length == 0)
                        {
                            continue;
                        }
                        ByteHelper.BytetoFile(by, str);
                    }
                    else
                    {
                        Log("客户端" + temp.RemoteEndPoint.ToString() + "：" + recvStr);
                    }
                }
                catch
                {
                    //MessageBox.Show("请连接服务器");
                    continue;
                }
            }
        }
        private void SendSocket()
        {
            string message = textBox3.Text;
            byte[] data = Encoding.UTF8.GetBytes(message);
            if (temp!=null)
            {
                temp.Send(data);//Send方法只能接受byte类型，于是先把数据转为byte类型。 
            }
        }
            #region AddTxtLine 添加提示信息
            public void Log(string msg, bool isErr = false)
        {

            BeginInvoke(new mydelegate(ShowRecvMessage), new object[] { msg, isErr });
            //LogUnit.Warn(msg);

        }

        public void ShowRecvMessage(string str, bool isErr = false)
        {
            AddTxtLine(str, false);
        }

        /// <summary>
        ///  添加提示信息
        /// </summary>
        /// <param name="ShowMsg"></param>
        /// <param name="IsErr"></param>
        private void AddTxtLine(string ShowMsg, bool IsErr = false)
        {


            if (this.RcTextBox.Lines.Count() > 1000)
            {
                this.RcTextBox.Clear();
            }
            if (IsErr)
            {

                this.RcTextBox.SelectionColor = Color.Red;//设置成蓝色  
            }
            else
            {

                this.RcTextBox.SelectionColor = Color.White;//设置黑色
            }
            this.RcTextBox.AppendText(ShowMsg + Environment.NewLine);

        }
        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            Socket.Close();
            temp.Close();
            thread.Abort();
            Log("关闭服务");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmClient frmClient = new FrmClient();
            frmClient.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SendSocket();
        }

        private void FrmServer_Load(object sender, EventArgs e)
        {
            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
            foreach (IPAddress ipa in ipadrlist)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                {
                    comboBox1.Items.Add(ipa.ToString()) ;
                }
            }
        }
    }
}
