using Infrastructure.Utility;
using Manage.NewMK.WebApi.App_Start.Attribute;
using NewMK.Domian.DomainException;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Manage.NewMK.WebApi.Controllers.Base
{
    [CommonApiException]
    public class ApiControllerBase: ApiController
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 当前系统用户
        /// </summary>
        public Guid CurrentUserId
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
 
    }
}