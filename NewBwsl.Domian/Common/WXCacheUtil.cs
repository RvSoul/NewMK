using Infrastructure.Utility;
using NewMK.Domian.DomainException;
using NewMK.Domian.Weixin;
using NewMK.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static NewMK.DTO.User.WeiXinAccess;

namespace NewMK.Domian.Common
{
    public class WXCacheUtil
    {
        /// <summary>
        /// 获取微信的AccessToken
        /// </summary>
        /// <returns></returns>
        public static WeiXinAccessAccess GetAccessToken()
        {
            var s = DataCache.GetCache(CacheKey.PRIX_WX_ACCESSTOKE);
            if (s == null)
            {
                return null;
            }
            return (WeiXinAccessAccess)s;
        }

        /// <summary>
        /// 设置微信的AccessToken
        /// </summary>
        /// <param name="value"></param>
        public static void SetAccessToken(WeiXinAccessAccess value)
        {
            DataCache.SetCache(CacheKey.PRIX_WX_ACCESSTOKE, value, DateTime.Now.AddMinutes(90));
        }

        /// <summary>
        /// 获取微信的JsApiTicket
        /// </summary>
        /// <returns></returns>
        public static WeiXinTicket GetJsApiTicket()
        {
            var s = DataCache.GetCache(CacheKey.PRIX_WX_JSAPITICKET);
            if (s == null)
            {
                return null;
            }
            return (WeiXinTicket)s;
        }

        /// <summary>
        /// 设置微信的JsApiTicket
        /// </summary>
        /// <param name="value"></param>
        public static void SetJsApiTicket(WeiXinTicket value)
        {
            DataCache.SetCache(CacheKey.PRIX_WX_JSAPITICKET, value, DateTime.Now.AddMinutes(90));
        }
    }
}