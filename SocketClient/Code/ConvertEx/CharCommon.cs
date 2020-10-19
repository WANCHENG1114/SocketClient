using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Utils.ConvertEx
{
    public class CharCommon
    {
        /// <summary>
        /// 10进制转换为二进制List
        /// </summary>
        /// <param name="value10"></param>
        /// <returns></returns>
        public static List<int> IntToBinaryList(int value10) {
            int curValue = value10;
            List<int> binaryLIst = new List<int>();
            while (curValue > 0) {
                binaryLIst.Add(curValue % 2);
                curValue = curValue / 2;
            } 
            return binaryLIst;
        }
        /// <summary>
        /// 列表倒序
        /// </summary>
        /// <param name="inList"></param>
        /// <returns></returns>
        public static List<int> ListReverse(List<int> inList)
        {
           
            List<int> binaryLIst = new List<int>();
            int icount = inList.Count;
            for (int x = icount-1; x >= 0; x--) {
                binaryLIst.Add(inList[x]);
            }
            return binaryLIst;
        }
    }
}
