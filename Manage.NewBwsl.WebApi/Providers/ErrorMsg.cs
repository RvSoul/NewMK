using System.Linq;
using System.Web.Http;

namespace Manage.NewMK.WebApi.Providers
{
    public class ErrorMsg : ApiController
    {
        /// <summary>
        /// 获取 web.mvc 对象验证的错误信息
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public string MvcMessage(System.Web.Mvc.ModelStateDictionary state)
        {
            string errmsg = string.Empty;
            foreach (var one in state)
            {
                if (one.Value.Errors.Count > 0)
                {
                    errmsg = one.Key + one.Value.Errors.First().ErrorMessage;
                    break;
                }
            }
            return errmsg;
        }

        /// <summary>
        /// 获取 web.http 对象验证的错误信息
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public string HttpMessage(System.Web.Http.ModelBinding.ModelStateDictionary state)
        {
            string errmsg = string.Empty;
            foreach (var one in state)
            {
                if (one.Value.Errors.Count > 0)
                {
                    errmsg = one.Key + one.Value.Errors.First().ErrorMessage;
                    break;
                }
            }
            return errmsg;
        }

        private string ModelStateMessage()
        {
            string errmsg = string.Empty;
            foreach (var key in ModelState.Keys)
            {

                var modelState = ModelState[key];
                if (modelState.Errors.Any())
                {
                    errmsg = modelState.Errors.FirstOrDefault().ErrorMessage;
                    break;
                }
            }
            return errmsg;
        }
    }
}