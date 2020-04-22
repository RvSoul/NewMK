using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.Pay.WxException
{
    /// <summary>
    /// 微信单状态异常，订单下单异常异常：商户订单已支付
    /// </summary>
    public class WxOrderPaidException : ApplicationException
    {
        public WxOrderPaidException() { }
        public WxOrderPaidException(string message)
            : base(message) { }
        public WxOrderPaidException(string message, Exception inner)
            : base(message, inner) { }
    }
}