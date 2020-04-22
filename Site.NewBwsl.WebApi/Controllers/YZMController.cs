using NewMK.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using NewMK.Domian.ThirdParty.lingkaiDX;
using System.Web.Caching;
using Site.NewMK.WebApi.Controllers.Base;

namespace Site.NewMK.WebApi.Controllers
{
    public class YZMController : ApiControllerBase
    {
         
        /// <summary>
        /// 发送验证码(所有用户)
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/AddYzm2")]
        public ResultEntity<bool> AddYzm2(string phone)
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
                HttpRuntime.Cache.Insert(phone + "-yzm", newRandom, null, DateTime.Now.AddMinutes(3), TimeSpan.Zero, CacheItemPriority.High, null);
                //HttpRuntime.Cache.Insert("yzm", newRandom.ToString());
                string aa = HttpRuntime.Cache[phone + "-yzm"].ToString();


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
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR;log.Error(error.Message, error);
            }
            return result;
        }
    }
}
