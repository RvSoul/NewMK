using Infrastructure.Utility;
using NewMK.Domian.Common;
using NewMK.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using WxPayAPI;
using static NewMK.DTO.User.WeiXinAccess;

namespace NewMK.Domian.Weixin
{
    public class AccessTokenUtil
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string GET_TOKEN_URL = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
        private static string GET_TICKET_URL = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi";

        private AccessTokenUtil()
        {

        }

        public static AccessTokenUtil Instance
        {
            get { return Nested.instance; }
        }

        /// <summary>
        /// 获取微信的AccessToken
        /// </summary>
        public string AaccessToken
        {
            get
            {
                var accessToken = WXCacheUtil.GetAccessToken();

                if (accessToken == null || !accessToken.ExprireTime.HasValue || 
                    accessToken.ExprireTime.Value.AddMinutes(-30) < DateTime.Now)//提前30分钟刷新
                {
                    Refresh();
                    accessToken = WXCacheUtil.GetAccessToken();
                }
                return accessToken.access_token;
            }
        }

        /// <summary>
        /// 获取微信的JsApiTicket
        /// </summary>
        public string JsApiTicket
        {
            get
            {
                var ticket = WXCacheUtil.GetJsApiTicket();

                if (ticket == null || !ticket.ExprireTime.HasValue || 
                    ticket.ExprireTime.Value.AddMinutes(-30) < DateTime.Now)//提前30分钟刷新
                {
                    if(ticket != null)
                    {
                        log.Info(ticket.ToJSON());
                    }
                    else
                    {
                        log.Info("Ticket为空！");
                    }
                    Refresh();
                    ticket = WXCacheUtil.GetJsApiTicket();
                }
                return ticket.ticket;
            }
        }

        /// <summary>
        /// 主动刷新AccessToken和JsApiTicket
        /// </summary>
        public void Refresh()
        {
            log.Debug("刷新AccessToken和JsApiTicket开始");
            var weiXinAccess = HttpHelper.GetHttpInfo<WeiXinAccessAccess>(string.Format(GET_TOKEN_URL, WxPayConfig.GetConfig().GetAppID(), WxPayConfig.GetConfig().GetAppSecret()));
            WXCacheUtil.SetAccessToken(weiXinAccess);

            var weiXinTicket = HttpHelper.GetHttpInfo<WeiXinTicket>(string.Format(GET_TICKET_URL, weiXinAccess.access_token));
            WXCacheUtil.SetJsApiTicket(weiXinTicket);
            log.Debug("刷新AccessToken和JsApiTicket结束");
        }

        private class Nested
        {
            internal static readonly AccessTokenUtil instance = null;

            static Nested()
            {
                instance = new AccessTokenUtil();
            }
        }
    }
}