using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pay
{
    class SomeHelpServer
    {
        /// <summary>
        /// 随机字符串
        /// </summary>
        /// <returns></returns>
        public static string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString().ToLower().Replace("-", "");
        }

        /// <summary>
        /// 拼接QueryString
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string BuildQueryString(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }
            return string.Format("&{0}={1}", name, value);
        }
    }
}
