using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.Pay.WxException
{
    /// <summary>
    /// 微信单状态异常，订单没有支付，若需要支付就需要重新下单。
    /// </summary>
    public class WxOrderNotPayException : ApplicationException
    {
        public WxOrderNotPayException() { }
        public WxOrderNotPayException(string message)
            : base(message) { }
        public WxOrderNotPayException(string message, Exception inner)
            : base(message, inner) { }
    }
}