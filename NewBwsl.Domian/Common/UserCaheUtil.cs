using Infrastructure.Utility;
using NewMK.Domian.DomainException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NewMK.Domian.Common
{
    public class UserCaheUtil
    {
        /// <summary>
        /// 用户是否重复点击
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static void Validate(string userId)
        {
            var key = $"{CacheKey.PRIX_USERKEY}{userId}";
            var dt = DateTime.Now;
            var s = DataCache.GetCache(key);
            if (s == null)
            {
                DataCache.SetCache(key, dt, DateTime.Now.AddMinutes(5));
                return;
            }
            dt = Convert.ToDateTime(s.ToString());
            TimeSpan dtc = DateTime.Now - dt;
            if (dtc.Seconds < 1)
            {
                throw new DMException("请不要重复操作！");
            }
        }
    }
}