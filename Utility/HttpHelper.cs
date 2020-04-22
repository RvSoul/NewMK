using Infrastructure.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Utility
{
    public class HttpHelper
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private CookieContainer cc;

        public CookieContainer CC
        {
            get
            {
                return cc;
            }
            set
            {
                this.cc = value;
            }
        }

        public HttpHelper()
        {
            this.cc = new CookieContainer();
        }

        public HttpHelper(CookieContainer cc)
        {
            this.cc = cc;
        }
        public static void Synchronous(string url, string param)
        {
            try
            {
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url + "?" + param);
                req.Method = "GET";
                req.ContentType = "application/x-www-form-urlencoded";
                req.GetResponse();
            }
            catch { }
        }

        /// <summary>
        /// http请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T GetHttpInfo<T>(string url)
        {
            log.Debug($"调用接口的Url：{url}");
            WebRequest myWebRequest = WebRequest.Create(url);
            using (WebResponse myWebResponse = myWebRequest.GetResponse())
            {
                using (Stream ReceiveStream = myWebResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(ReceiveStream, Encoding.UTF8))
                    {
                        var s = reader.ReadToEnd();
                        log.Debug($"接口Url返回信息：{s}");
                        return JsonHelper.FromJSON<T>(s);
                    }
                }
            }
        }
        public static string GetHttpInfo(string url)
        {
            log.Debug($"调用接口的Url：{url}");
            WebRequest myWebRequest = WebRequest.Create(url);
            using (WebResponse myWebResponse = myWebRequest.GetResponse())
            {
                using (Stream ReceiveStream = myWebResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(ReceiveStream, Encoding.UTF8))
                    {
                        var s = reader.ReadToEnd();
                        log.Debug($"接口Url返回信息：{s}");
                        return s;
                    }
                }
            }
        }

        #region 读取或写入cookie
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = UrlEncode(strValue);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string key, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie[key] = UrlEncode(strValue);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string key, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie[key] = UrlEncode(strValue);
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">过期时间(分钟)</param>
        public static void WriteCookie(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = UrlEncode(strValue);
            cookie.Expires = DateTime.Now.AddYears(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
                return UrlDecode(HttpContext.Current.Request.Cookies[strName].Value.ToString());
            return "";
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName, string key)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null && HttpContext.Current.Request.Cookies[strName][key] != null)
                return UrlDecode(HttpContext.Current.Request.Cookies[strName][key].ToString());

            return "";
        }
        #endregion

        #region URL处理
        /// <summary>
        /// URL字符编码
        /// </summary>
        public static string UrlEncode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            str = str.Replace("'", "");
            return HttpContext.Current.Server.UrlEncode(str);
        }

        /// <summary>
        /// URL字符解码
        /// </summary>
        public static string UrlDecode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            return HttpContext.Current.Server.UrlDecode(str);
        }
        #endregion

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }

        public static string HttpsRequest(string requestUrl, string requestMethod, string outputStr)
        {
            //System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            string result = "";//返回结果
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (requestUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(requestUrl);

                request.Method = requestMethod;
                request.Timeout = 10000;

                if (requestMethod == "POST")
                {
                    //设置POST的数据类型和长度
                    request.ContentType = "application/x-www-form-urlencoded";
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(outputStr);
                    request.ContentLength = data.Length;

                    //往服务器写入数据
                    reqStream = request.GetRequestStream();
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();

                //获取服务端返回数据
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = sr.ReadToEnd().Trim();
                }
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                    response.Dispose();
                }
                if (request != null)
                {
                    request.Abort();
                }
                if (reqStream != null)
                {
                    reqStream.Close();
                    reqStream.Dispose();
                }
            }
            return result;
        }

        /// <summary>
        /// 用MD5方式加密字符串
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5(string password)
        {
            ASCIIEncoding objAsc = new ASCIIEncoding();
            MD5 objMD5 = new MD5CryptoServiceProvider();
            byte[] arrRndHashPwd = objMD5.ComputeHash(objAsc.GetBytes(password));
            string strRndHashPwd = "";
            foreach (byte b in arrRndHashPwd)
            {
                if (b < 16)
                    strRndHashPwd = strRndHashPwd + "0" + b.ToString("X");
                else
                    strRndHashPwd = strRndHashPwd + b.ToString("X");
            }
            return strRndHashPwd;
        }

        /// <summary>
        /// Post 数据至服务端
        /// </summary>
        /// <param name="postContent">提交内容</param>
        /// <param name="url">提交地址</param>
        /// <returns></returns>
        public static string HttpPost(string postContent, string url)
        {
            return Post(url, Encoding.GetEncoding("GBK"), "text/xml", postContent, 80000);
        }

        public static string HttpPost(string postContent, string url, int timeout)
        {
            return Post(url, Encoding.UTF8, "application/x-www-form-urlencoded", postContent, timeout);
        }

        /// <summary>
        /// post方式发起请求，提交数据
        /// </summary>
        /// <param name="postData">提交内容</param>
        /// <param name="uri">提交地址</param>
        /// <param name="encode">提交时候使用的编码方式</param>
        /// <returns></returns>
        public static string Post(string uri, Encoding encode, string contentType, string postData, int timeout)
        {
            string strResult = string.Empty;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.Method = "POST";
            req.Timeout = timeout;
            req.Accept = "*/*";
            req.ContentType = contentType;
            Byte[] bytes = encode.GetBytes(postData);
            req.ContentLength = bytes.Length;
            HttpWebResponse resp = null;

            try
            {
                using (Stream sendStream = req.GetRequestStream())
                {
                    sendStream.Write(bytes, 0, bytes.Length);
                    sendStream.Flush();
                    sendStream.Close();
                }

                resp = (HttpWebResponse)req.GetResponse();

                using (Stream getStream = resp.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(getStream, Encoding.UTF8))
                    {
                        strResult = sr.ReadToEnd();
                        sr.Close();
                    }
                    getStream.Close();
                }
            }
            finally
            {
                if (resp != null)
                {
                    resp.Close();
                }
            }
            return strResult;
        }

        /// <summary>
        /// 获取客户端Ip地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUserIp()
        {
            /*
            var ip = context.Request.Headers["X-Real-IP"];
            log.Info($"X-Real-IP{ip},X-Forwarded-For{context.Request.Headers["X-Forwarded-For"]},UserHostAddress{context.Request.UserHostAddress}");
            if (!string.IsNullOrEmpty(ip) && !ip.ToLower().Equals("unknown"))
                return ParseIp(ip);
            */
            var ip = HttpContext.Current.Request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrEmpty(ip) && !ip.ToLower().Equals("unknown"))
                return ParseIp(ip);
            ip = HttpContext.Current.Request.Headers["Proxy-Client-IP"];
            if (!string.IsNullOrEmpty(ip) && !ip.ToLower().Equals("unknown"))
                return ParseIp(ip);
            ip = HttpContext.Current.Request.Headers["WL-Proxy-Client-IP"];
            if (!string.IsNullOrEmpty(ip) && !ip.ToLower().Equals("unknown"))
                return ParseIp(ip);
            ip = HttpContext.Current.Request.UserHostAddress;
            return ParseIp(ip);
        }

        private static string ParseIp(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return null;
            string[] ss = ip.Split(',');
            if (ss.Length == 0)
                return null;
            return ss[0];
        }
    }
}