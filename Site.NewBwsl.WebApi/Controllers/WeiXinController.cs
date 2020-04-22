using NewMK.DTO;
using NewMK.DTO.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Security;
using WxPayAPI;
using Site.NewMK.WebApi.Models;
using NewMK.Domian.DM;
using Site.NewMK.WebApi.Controllers.Base;
using NewMK.Domian.Common;
using Utility;
using NewMK.Domian.Weixin;
using NewMK.Domian.DomainException;

namespace Site.NewMK.WebApi.Controllers
{
    public class WeiXinController : ApiControllerBase
    {
        /// <summary>
        /// 获取微信分享信息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetTicket")]
        public ResultEntity<SignPackage> GetTicket(string url)
        {
            long time = Utils.ConvertTimeStamp(DateTime.Now);
            string nonceStr = Utils.createNonceStr();
            string raw = "jsapi_ticket=" + AccessTokenUtil.Instance.JsApiTicket + "&noncestr=" + nonceStr + "&timestamp=" + time + "&url=" + url;
            string signature = Utils.Sha1Sign(raw);
            return new ResultEntityUtil<SignPackage>().Success(new SignPackage()
            {
                appId = WxPayConfig.GetConfig().GetAppID(),
                nonceStr = nonceStr,
                timestamp = time.ToString(),
                url = url,
                signature = signature,
                rawString = raw
            });
        }

        /// <summary>
        /// 根据code获取微信用户基本信息
        /// </summary>
        /// <param name="code">code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetWxUserInfo")]
        public ResultEntity<WeiXinUserinfo> GetWxUserInfo(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + AppID + "&secret=" + AppSecret + "&code=" + code + "&grant_type=authorization_code";
            var accessToken = HttpHelper.GetHttpInfo<WeiXinAccess>(url);
            if (accessToken.errcode > 0)
            {
                throw new DMException($"调用微信授权失败，错误码：{accessToken.errcode}，错误信息：{accessToken.errmsg}");
            }

            url = "https://api.weixin.qq.com/sns/userinfo?access_token=" + accessToken.access_token + "&openid=" + accessToken.openid + "&lang=zh_CN";
            return new ResultEntityUtil<WeiXinUserinfo>().Success(HttpHelper.GetHttpInfo<WeiXinUserinfo>(url));
        }

        /// <summary>
        /// 获取微信的用户基本信息
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetWeiXinUserinfoUnionID")]
        public ResultEntity<WeiXinUserinfoUnionID> GetWeiXinUserinfoUnionID(string openid)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/user/info?access_token=" + AccessTokenUtil.Instance.AaccessToken + "&openid=" + openid + "&lang=zh_CN";
            return new ResultEntityUtil<WeiXinUserinfoUnionID>().Success(HttpHelper.GetHttpInfo<WeiXinUserinfoUnionID>(url));
        }

        [HttpGet]
        [Route("api/GetOpenIDPC")]
        public ResultEntity<string> GetOpenIDPC(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + AppID2 + "&secret=" + AppSecret2 + "&code=" + code + "&grant_type=authorization_code";
            return new ResultEntityUtil<string>().Success(HttpHelper.GetHttpInfo<string>(url));

        }

        /// <summary>
        /// 绑定微信OpenId
        /// </summary>
        /// <param name="usercode"></param> 
        /// <param name="openid"></param> 
        /// <returns></returns>
        [HttpGet]
        [Route("api/BDUserOpenId")]
        public ResultEntity<bool> BDUserOpenId(string usercode, string openid)
        {
            UserDM dm = new UserDM();
            string OpenID = openid;
            bool bo = dm.BDUserOpenId(usercode, OpenID);
            if (OpenID != "" && OpenID != null)
            {
                SendTempletMessge(OpenID, "", "");
            }
            return new ResultEntityUtil<bool>().Success(bo);

        }

        /// <summary>
        /// 消息推送
        /// </summary>
        /// <param name="OpenID"></param>
        /// <param name="msg"></param> 
        /// <param name="msg2"></param>
        public void SendTempletMessge(string OpenID, string msg, string msg2)
        {
            //微信获取openID
            #region 组装信息推送，并返回结果（其它模版消息于此类似）
            string url = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + AccessTokenUtil.Instance.AaccessToken;
            string temp = "{\"touser\": \"" + OpenID + "\"," +
                          "\"template_id\": \"TiRgUqiwW8hZspx58IQ_wfDcn7RqpsNS6uwfmwM4bl4\", " +
                          "\"topcolor\": \"#FF0000\", " +
                          "\"data\": " +
                          "{\"first\": {\"value\": \"" + msg + "\"}," +
                          "\"keyword1\": { \"value\": \"" + msg2 + "\"}," +
                          "\"keyword2\": { \"value\": \"" + DateTime.Now + "\"}," +
                          "\"remark\": {\"value\": \"详情请进入公众号内查看！\" }}}";

            #endregion

            //核心代码
            GetResponseData(temp, @url);
            //strReturn = "推送成功";
        }

        /// <summary>  
        /// 返回JSon数据  
        /// </summary>  
        /// <param name="JSONData">要处理的JSON数据</param>  
        /// <param name="Url">要提交的URL</param>  
        /// <returns>返回的JSON处理字符串</returns>  
        public string GetResponseData(string JSONData, string Url)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(JSONData);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentLength = bytes.Length;
            request.ContentType = "json";
            using (Stream reqstream = request.GetRequestStream())
            {
                reqstream.Write(bytes, 0, bytes.Length);
                //声明一个HttpWebRequest请求  
                request.Timeout = 90000;
                //设置连接超时时间  
                request.Headers.Set("Pragma", "no-cache");
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream streamReceive = response.GetResponseStream())
                    {
                        Encoding encoding = Encoding.UTF8;
                        using (StreamReader streamReader = new StreamReader(streamReceive, encoding))
                        {
                            string strResult = streamReader.ReadToEnd();
                            return strResult;
                        }
                    }
                }
            }
        }
    }
}