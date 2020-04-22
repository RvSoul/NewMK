using NewMK.Domian.DM;
using NewMK.Domian.ThirdParty.lingkaiDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace NewMK.Domian.ThirdParty
{
    public class SmsUtility
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string lksdkName = ReadConfig.GetConnectionString("lksdkName");
        private static string lksdkPwd = ReadConfig.GetConnectionString("lksdkPwd");
        private static string lksdk = ReadConfig.GetConnectionString("lksdk");

        #region 短信内容模板

        /// <summary>
        /// 自动退款短信
        /// </summary>
        private static string TEMP_AUTO_REFUND = "温馨提醒您：您注册的编码{0}因操作错误，订单未能生成。退款将在24小时内原路返回，请注意查看。";

        #endregion

        /// <summary>
        /// 发送欢迎短信
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="deLevelID"></param>
        /// <param name="userCode"></param>
        /// <param name="userName"></param>
        public static void SendWelcomeSMS(string phone, int deLevelID, string userCode, string userName)
        {
            OrderDM dm = new OrderDM();
            SendSMS(phone, dm.GetLoginDxInfo(deLevelID, userCode, userName));
        }

        /// <summary>
        /// 发送自动退款短信
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="userCode"></param>
        public static void SendAutoRefund(string phone,string userCode)
        {
            SendSMS(phone, string.Format(TEMP_AUTO_REFUND, userCode));
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="content">内容</param>
        public static void SendSMS(string phone, string content)
        {
            LinkWS wss = new LinkWS(lksdk);
            int r = wss.BatchSend(lksdkName, lksdkPwd, phone, content, "", "");
            log.Info($"短信发送，手机：{phone}，内容：{content}，网关返回码：{r}");
        }
    }
}
