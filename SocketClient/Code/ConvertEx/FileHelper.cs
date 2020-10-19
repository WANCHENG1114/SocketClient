using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketClienTool.Code.ConvertEx
{
    class FileHelper
    {
        string FileName;
        //默认打开路径
        private static string InitialDirectory = "D:\\";
        //统一对话框
        public static bool InitialDialog(FileDialog fileDialog, string title)
        {
            fileDialog.InitialDirectory = InitialDirectory;//初始化路径
            fileDialog.Filter = "txt files (*.txt,*.*)|*.txt;*.*";//过滤选项设置，文本文件，所有文件。
            fileDialog.FilterIndex = 1;//当前使用第二个过滤字符串
            fileDialog.RestoreDirectory = true;//对话框关闭时恢复原目录
            fileDialog.Title = title;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 1; i <= fileDialog.FileName.Length; i++)
                {
                    if (fileDialog.FileName.Substring(fileDialog.FileName.Length - i, 1).Equals(@"\"))
                    {
                        //更改默认路径为最近打开路径
                        InitialDirectory = fileDialog.FileName.Substring(0, fileDialog.FileName.Length - i + 1);
                        return true;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        public void Open(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();//打开文件对话框              
            if (InitialDialog(openFileDialog, "Open"))
            {
                using (Stream stream = openFileDialog.OpenFile())
                {
                    FileName = ((System.IO.FileStream)stream).Name;
                    // 执行相关文件操作

                }
            }
        }
        public void Save(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();//打开文件对话框              
            if (InitialDialog(saveFileDialog, "Save"))
            {
                using (Stream stream = saveFileDialog.OpenFile())
                {
                    FileName = ((System.IO.FileStream)stream).Name;
                    //执行保存动作

                    MessageBox.Show("保存成功。");
                }
            }
        }
        /// <summary>
        /// 判断文件类型
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string JudgeFileType(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            string fileclass = "";
            try
            {

                for (int i = 0; i < 2; i++)
                {
                    fileclass += reader.ReadByte().ToString();
                }

            }
            catch (Exception)
            {
                throw;
            }
            return Enum.Parse(typeof(FileExtensione), fileclass.ToString()).ToString();
        }
        public enum FileExtensione
        {
            JPG = 255216,
            GIF = 7173,
            BMP = 6677,
            TIFF = 7373,
            PNG = 13780,
            COM = 7790,
            EXE = 7790,
            DLL = 7790,
            RAR = 8297,
            ZIP = 8075,
            XML = 6063,
            HTML = 6033,
            ASPX = 239187,
            CS = 117115,
            JS = 119105,
            //txt = 210187/ 4946/ 104116,
            txt = 210187,
            SQL = 255254,
            BAT = 64101,
            BTSEED = 10056,
            RDP = 255254,
            PSD = 5666,
            PDF = 3780,
            CHM = 7384,
            LOG = 70105,
            REG = 8269,
            HLP = 6395,
            DOC = 208207,
            XLS = 208207,
            DOCX = 208207,
            XLSX = 208207,
        }
    }
}
