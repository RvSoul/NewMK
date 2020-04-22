using Infrastructure.Utility;
using NewMK.Domian.DomainException;
using NewMK.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace Manage.NewMK.WebApi.App_Start.Attribute
{
    /// <summary>
    /// API自定义错误过滤器属性
    /// </summary>
    public class CommonApiExceptionAttribute: ExceptionFilterAttribute
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 统一对调用异常信息进行处理，返回自定义的异常信息
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(HttpActionExecutedContext context)
        {
            ResultEntity<bool> result = new ResultEntity<bool>();
            if (context.Exception is UnAuthorizeException)
            {               
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.UnAuthorize);
                result.Msg = context.Exception.Message;                
            }
            else if (context.Exception is DMException)
            {
                log.Debug(context.Exception.Message, context.Exception);
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = context.Exception.Message;
            }
            else if (context.Exception is EntityException)
            {
                log.Debug(context.Exception.Message, context.Exception);
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = context.Exception.Message;
            }
            else
            {
                log.Error(context.Exception.Message, context.Exception);
                result.ErrorCode = Convert.ToInt32(Utility.ApiResultCode.Error);
                result.Msg = Utility.ApiResultMessage.MESSAGE_ERROR;
            }
            HttpResponseMessage rsp = new HttpResponseMessage { Content = new StringContent(result.ToJSON(), Encoding.GetEncoding("UTF-8"), "application/json") };
            context.Response = rsp;

            base.OnException(context);
        }
    }
}