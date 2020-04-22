using Infrastructure.Utility;
using Manage.NewMK.WebApi.Controllers.Base;
using NewMK.Domian.Common;
using NewMK.Domian.ThirdParty.lingkaiDX;
using NewMK.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Http;

namespace Manage.NewMK.WebApi.Controllers
{
    [Authorize]
    public class YZMController : ApiControllerBase
    {

        /// <summary>
        /// 发送验证码，不需要授权,五分钟有效
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddYzm")]
        [AllowAnonymous]
        public ResultEntity<bool> AddYzm(string phone)
        {
            return NewMethod(phone, "", 5);
        }

        /// <summary>
        /// 发送验证码，需要授权，一个小时有效
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddYzm2")]
        public ResultEntity<bool> AddYzm2(string phone)
        {
            return NewMethod(phone, CurrentUserId.ToString(), 60);
        }

        private static ResultEntity<bool> NewMethod(string phone, string userId, double time)
        {
            ResultEntity<bool> result = new ResultEntity<bool>();
            try
            {

                char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                StringBuilder newRandom = new StringBuilder();
                Random rd = new Random();
                for (int i = 0; i < 4; i++)
                {
                    newRandom.Append(constant[rd.Next(10)]);
                }
                var key = $"{CacheKey.PRIX_USERKEY}{userId}";
                DataCache.SetCache(key, newRandom, DateTime.Now.AddMinutes(time));

                LinkWS WSS = new LinkWS(ConfigurationManager.ConnectionStrings["lksdk"].ConnectionString);
                int R = WSS.BatchSend(
                    ConfigurationManager.ConnectionStrings["lksdkName"].ConnectionString,
                    ConfigurationManager.ConnectionStrings["lksdkPwd"].ConnectionString,
                    phone,
                    "您的手机验证码为：" + newRandom.ToString() + "，请勿把验证码泄露给他人。", "", "");
                if (R == 1)
                {
                    //result.Data = ResultEntity<true>;
                    result.IsSuccess = true;
                    result.Count = 0;
                    result.Msg = "短信发送成功！";
                }
                else
                {
                    result.ErrorCode = 113;
                    result.Msg = "短信发送失败！";
                }
            }
            catch (Exception error)
            {
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR; log.Error(error.Message, error);
            }
            return result;
        }
    }
}
