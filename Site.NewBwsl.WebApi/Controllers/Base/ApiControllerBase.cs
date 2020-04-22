using Infrastructure.Utility;
using Manage.NewMK.WebApi.App_Start.Attribute;
using NewMK.Domian.DomainException;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;
using WxPayAPI;

namespace Site.NewMK.WebApi.Controllers.Base
{
    [CommonApiException]
    public class ApiControllerBase: ApiController
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 当前系统用户
        /// </summary>
        protected Guid CurrentUserId
        {
            get
            {
                if (RequestContext.Principal.Identity.Name != null)
                {
                    return new Guid(RequestContext.Principal.Identity.Name);
                }
                throw new UnAuthorizeException("用户未登录");
            }
        }

        /// <summary>
        /// 公众号AppID
        /// </summary>
        protected string AppID = WxPayConfig.GetConfig().GetAppID();
        
        /// <summary>
        /// 公众号AppSecret
        /// </summary>
        protected string AppSecret = WxPayConfig.GetConfig().GetAppSecret();
        /// <summary>
        /// 开放平台AppID
        /// </summary>
        protected string AppID2 = "wx8a39913442e8173d";
        /// <summary>
        /// 开放平台AppSecret
        /// </summary>
        protected string AppSecret2 = "f718c49118f0234abc476302e219a65f";

        protected string KuaiQianZhanghao = ConfigurationManager.AppSettings["KuaiQianZhanghao"].ToString();
        protected string KuaiQianHuidiao = ConfigurationManager.AppSettings["KuaiQianHuidiao"].ToString();
        protected string KuaiQianMima = ConfigurationManager.AppSettings["KuaiQianMima"].ToString();
        protected string KuaiQianZhengshu = ConfigurationManager.AppSettings["KuaiQianZhengshu"].ToString();
    }
}