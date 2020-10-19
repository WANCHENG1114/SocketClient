using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClienTool.Code.ConvertEx
{
    public class ByteHelper
    {
        public static string IntToHex(int i)
        {
            return Convert.ToString(i, 16);
        }
        /// <summary>
        /// int 类型转回为 byte[]
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte[] IntToByte2(int i)
        {
            byte[] a = new byte[2];
            a[0] = (byte)(i >> 8);
            a[1] = (byte)i;
            return a;
        }
        /// <summary>
        /// 截取byte 数组
        /// </summary>
        /// <param name="frombyte"></param>
        /// <param name="begIndex"></param>
        /// <param name="lenth"></param>
        /// <returns></returns>
        public static  byte[] SubArr(byte[] frombyte, int begIndex, int lenth)
        {
            byte[] subByte = new byte[lenth];
            int icount = (begIndex + lenth);
            for (int x = begIndex; x< icount; x++) {
                subByte[x - begIndex] = frombyte[x];
            }
            return subByte;
        }
        public static string SubToString(byte[] frombyte, int begIndex, int lenth)
        {
            byte[] subByte = new byte[lenth];
            int icount = (begIndex + lenth);
            for (int x = begIndex; x < icount; x++)
            {
                subByte[x - begIndex] = frombyte[x];
            }
            return Encoding.UTF8.GetString(subByte);
        }
        
        /// <summary>
        /// 数组相加
        /// </summary>
        /// <param name="bytes1"></param>
        /// <param name="bytes2"></param>
        /// <returns></returns>
        public static byte[] Add(byte[]  bytes1, byte[] bytes2)
        {
            int len1 = bytes1.Length;
            int len2 = bytes2.Length;
            int allLen = len1 + len2;
            byte[] newByte = new byte[allLen];
           
            for (int x = 0; x < len1; x++)
            {
                newByte[x] = bytes1[x];
            }
            for (int x = 0; x < len2; x++)
            {
                newByte[x+len1] = bytes2[x];
            }
            return newByte;
        }
        /// <summary>
        /// subbyte 合并到 bytesA
        /// </summary>
        /// <param name="bytesA">原始数组</param>
        /// <param name="subbytes">添加数组</param>
        /// <param name="index">添加起开始位置</param>
        /// <returns></returns>
        public static byte[] Append(byte[] bytesA, byte[] subbytes,int index)
        {
            int len1 = subbytes.Length;
           

            for (int x = 0; x < len1; x++)
            {
                bytesA[x+index] = subbytes[x];
            }
            
            return bytesA;
        }
        public static byte[] Append(byte[] bytesA, string substr, int index)
        {
            int len1 = substr.Length;

            byte[] subbytes = Encoding.UTF8.GetBytes(substr);
            for (int x = 0; x < len1; x++)
            {
                bytesA[x + index] = subbytes[x];
            }

            return bytesA;
        }
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i<s.Length; i += 2)
            {
                buffer[i / 2] = (byte) Convert.ToByte(s.Substring(i, 2), 16);
            }

            return buffer;
        }

        /// <summary> Converts an array of bytes into a formatted string of hex digits (ex: E4 CA B2)</summary>
        /// <param name="data"> The array of bytes to be translated into a string of hex digits. </param>
        /// <returns> Returns a well formatted string of hex digits with spacing. </returns>
        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            }

            return sb.ToString().ToUpper();
        }


        /// <summary>
        /// 读文件到byte[]
        /// </summary>
        /// <param name="fileName">硬盘文件路径</param>
        /// <returns></returns>
        public static byte[] ReadFileToByte(string fileName)
        {
            FileStream pFileStream = null;
            byte[] pReadByte = new byte[0];
            try
            {
                pFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(pFileStream);
                r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
                pReadByte = r.ReadBytes((int)r.BaseStream.Length);
                return pReadByte;
            }
            catch
            {
                return pReadByte;
            }
            finally
            {
                if (pFileStream != null)
                    pFileStream.Close();
            }
        }

        /// <summary>
        /// 写byte[]到fileName
        /// </summary>
        /// <param name="pReadByte">byte[]</param>
        /// <param name="fileName">保存至硬盘路径</param>
        /// <returns></returns>
        public static bool WriteByteToFile(byte[] pReadByte, string fileName)
        {
            FileStream pFileStream = null;
            try
            {
                pFileStream = new FileStream(fileName, FileMode.OpenOrCreate);
                pFileStream.Write(pReadByte, 0, pReadByte.Length);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (pFileStream != null)
                    pFileStream.Close();
            }
            return true;
        }
        /// <summary>
        /// 文件转byte
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] FileStoByte(string fileName)
        {
            FileStream stream = new FileInfo(fileName).OpenRead();
            Byte[] buffer = new Byte[stream.Length];
            //从流中读取字节块并将该数据写入给定缓冲区buffer中
            stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
            return buffer;
        }
        /// <summary>
        /// byte转文件
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="filename"></param>
        public static void BytetoFile(byte[] buffer, string filename)
        {
            try
            {
                using (FileStream fss = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    fss.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        /// <summary>
        /// 将文件转换成byte[] 数组
        /// </summary>
        /// <param name="fileUrl">文件路径文件名称</param>
        /// <returns>byte[]</returns>

        protected byte[] AuthGetFileData(string fileUrl)
        {
            using (FileStream fs = new FileStream(fileUrl, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                byte[] buffur = new byte[fs.Length];
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(buffur);
                    bw.Close();
                }
                return buffur;
            }
        }
    }
}
