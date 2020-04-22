using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO
{
    public static class DataText
    {
        public static string ToString2(this object str)
        {
            try
            {
                if (str.ToString()== "00000000-0000-0000-0000-000000000000")
                {
                    return null;
                }
                else
                {
                    return str.ToString();
                }
                
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 转换为int 转换失败返回null
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int? intnull(this object val)
        {
            try
            {
                return Convert.ToInt32(val);
            }
            catch (Exception)
            {
                return (int?)null;
            }
        }

        /// <summary>
        /// 转换为int 转换失败返回0
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int int0(this object val)
        {
            try
            {
                return Convert.ToInt32(val);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
