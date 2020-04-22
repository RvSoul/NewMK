using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.Pay.WxException
{
    /// <summary>
    /// 微信单状态异常，订单不存在异常
    /// </summary>
    public class WxOrderNotExistsException : ApplicationException
    {
        public WxOrderNotExistsException() { }
        public WxOrderNotExistsException(string message)
            : base(message) { }
        public WxOrderNotExistsException(string message, Exception inner)
            : base(message, inner) { }
    }
}