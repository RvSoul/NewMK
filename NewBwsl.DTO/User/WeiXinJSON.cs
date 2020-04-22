using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.User
{
    /// <summary>
    /// 网页授权的临时AccessToken
    /// </summary>
    public class WeiXinAccess
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }

        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string UnionID { get; set; }
        public string scope { get; set; }

        /// <summary>
        /// 微信的AccessToken，2小时有效期
        /// </summary>
        public class WeiXinAccessAccess
        {
            public int errcode { get; set; }
            public string errmsg { get; set; }

            public string access_token { get; set; }
            public int expires_in { get; set; }

            /// <summary>
            /// 凭证过期时间
            /// </summary>
            public DateTime? ExprireTime
            {
                get
                {
                    if (expires_in == 0)
                        return null;
                    DateTime dt = DateTime.Now;
                    return dt.AddSeconds(expires_in);
                }
            }
        }
    }
    /// <summary>
    /// 微信的api_ticket，2小时过期
    /// </summary>
    public class WeiXinTicket
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string ticket { get; set; }
        public int expires_in { get; set; }

        public DateTime? ExprireTime
        {
            get
            {
                if (expires_in == 0)
                    return null;
                DateTime dt = DateTime.Now;
                return dt.AddSeconds(expires_in);
            }
        }
    }

    public class WeiXinUserinfo
    {
        public string openid { get; set; }
        public string nickname { get; set; }
        public string sex { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
        public string[] privilege { get; set; }
        public string unionid { get; set; }

    }
    public class WeiXinUserinfoUnionID
    {
        public string subscribe { get; set; }
        public string openid { get; set; }
        public string nickname { get; set; }
        public string sex { get; set; }
        public string language { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
        public string subscribe_time { get; set; }
        public string unionid { get; set; }
        public string remark { get; set; }
        public string groupid { get; set; }
        public string[] tagid_list { get; set; }
        public string subscribe_scene { get; set; }
        public string qr_scene { get; set; }
        public string qr_scene_str { get; set; }

    }

    public class WeiXinErr
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
    }
}
