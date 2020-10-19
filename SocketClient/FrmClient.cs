using SocketClienTool.Code.ConvertEx;
using SocketClienTool.Code.SocketHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketClienTool
{
    public partial class FrmClient : Form
    {
        public delegate void mydelegate(string str, bool isErr = false);
        SocketClient Socket_Client = null;
        Thread Htask = null;
       
        public FrmClient()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string ip = txtIp.Text;
            int port = 0;
            int.TryParse(txtPort.Text,out port);
            Socket_Client = new SocketClient(ip,port);
            Socket_Client.OnRecData +=SocketServerDataReceived;
            bool startOk=  Socket_Client.Start();
            if (startOk)
            {
                Log("链接成功！");
                Htask = new Thread(new ThreadStart(SendToHeartBeat));
                Htask.Start();
                SetButtonState(0);
            }
            else {
                Log("链接失败！");
            }
            
        }
        

        private void btnStop_Click(object sender, EventArgs e)
        {
            Htask.Abort();
            Socket_Client.OnRecData -= SocketServerDataReceived;
            Socket_Client.Stop();

            Log("断开链接！");
            SetButtonState(1);
        }
        
        /// <summary>
        /// Socket Server 接收到数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cmdData"></param>
        public void SocketServerDataReceived(Object sender, byte[] data)
        {
            string cmdData = System.Text.Encoding.UTF8.GetString(data);
            Log("接收服务端："+cmdData);
             

        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            string sendTxt = txtSend.Text;
            if (string.IsNullOrWhiteSpace(sendTxt))
            {
                MessageBox.Show("不能发送空内容");
            }
            if (Socket_Client!=null)
            {
                Socket_Client.SendToService(sendTxt);
            }
        }

        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();//打开文件对话框              
            if (FileHelper.InitialDialog(openFileDialog, "选择文件"))
            {
                using (Stream stream = openFileDialog.OpenFile())
                {
                    string FileName = ((System.IO.FileStream)stream).Name;
                    var filebyte = ByteHelper.FileStoByte(FileName);
                    
                    int byteSize= filebyte.Length;//文件大小
                    //string fileType = FileHelper.JudgeFileType(stream);//文件类型
                    string fileType = FileName.Split(new char[] {'.' })[1];//文件类型
                    byte[] bsnew = new byte[1024];
                    byte[] bs = new byte[1024];
                    bs = Encoding.UTF8.GetBytes("###,"+byteSize+","+ fileType);   //把字符串编码为字节
                  

                    Array.Copy(bs, bsnew, bs.Length);
                    // 执行相关文件操作
                    var li = bsnew.ToList();
                    li.AddRange(filebyte);

                    filebyte = li.ToArray();

                    if (Socket_Client!=null)
                    {
                        Socket_Client.SendToService(filebyte);
                    }
                }
            }
        }
        
        /// <summary>
        /// 发送心跳
        /// </summary>
        private void SendToHeartBeat()
        {
            string HeartBeatStr = "0#";
            while (true)
            {
                try
                {
                    Socket_Client.SendToService(HeartBeatStr);
                }
                catch (Exception ex)
                {
                    Log(txtIp.Text + " 链接失败 " + ex.Message);
                }
                Thread.Sleep(5000);
            }
        }
        private void SetButtonState(int xflag)
        {
            if (xflag == 0)//btnStart
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;

            }
            else //btnstop
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;

            }

        }
        #region AddTxtLine 添加提示信息
        public void Log(string msg, bool isErr = false)
        {

            BeginInvoke(new mydelegate(ShowRecvMessage), new object[] { msg, isErr });
            //LogUnit.Warn(msg);

        }

        public  void ShowRecvMessage(string str, bool isErr = false)
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

       

        private void button1_Click(object sender, EventArgs e)
        {
           
            for (int i = 0; i < 300; i++) {
                string com = FormatCom.LeftzeroStr( (i / 40) + 1,2);
                string addr = FormatCom.LeftzeroStr((i % 40) + 1, 2);

               // string cmd1 = $"groupName1,LPC0.COM{com}.ADDR{addr}.Doffer.doffer_output";
                string cmd1 = $"WD,LPC0.COM{com}.ADDR{addr}.Doffer.doffer_output,31";
              //  string cmd1 = $"WD,LPC0.COM{com}.ADDR{addr}.Doffer.doffer_output,22";
               // string cmd2 = $"WD,LPC0.COM{com}.ADDR{addr}.Doffer.winder_output,33";
               // Log(cmd1);
                Socket_Client.SendToService(cmd1);
                Thread.Sleep(50);
                //Socket_Client.SendToService(cmd2);
                //Thread.Sleep(10);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 300; i++)
            {
                string com = FormatCom.LeftzeroStr((i / 40) + 1, 2);
                string addr = FormatCom.LeftzeroStr((i % 40) + 1, 2);
                //string cmd1 = $"WD,LPC0.COM{com}.ADDR{addr}.Doffer.doffer_output,30";
                string cmd1 = $"RD,LPC0.COM{com}.ADDR{addr}.Doffer.doffer_output,0";
               
              //  Log(cmd1);
                Socket_Client.SendToService(cmd1);
                Thread.Sleep(50);
               
            }
        }

        private void button4_Click(object sender, EventArgs e)
        { OpenFileDialog openFileDialog = new OpenFileDialog();//打开文件对话框              
            if (FileHelper.InitialDialog(openFileDialog, "选择文件"))
            {
                using (Stream stream = openFileDialog.OpenFile())
                {
                    string FileName = ((System.IO.FileStream)stream).Name;
                    byte[] filebyte= ByteHelper.ReadFileToByte(FileName);
                    if (Socket_Client != null)
                    {
                        Socket_Client.SendToService(filebyte);
                    }
                }
            }
                   
        }
    }
}
