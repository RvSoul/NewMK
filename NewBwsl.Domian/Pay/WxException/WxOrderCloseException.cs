using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.Pay.WxException
{
    /// <summary>
    /// 微信单状态异常，订单下单异常异常：订单已关闭
    /// </summary>
    public class WxOrderCloseException : ApplicationException
    {
        public WxOrderCloseException() { }
        public WxOrderCloseException(string message)
            : base(message) { }
        public WxOrderCloseException(string message, Exception inner)
            : base(message, inner) { }
    }
}