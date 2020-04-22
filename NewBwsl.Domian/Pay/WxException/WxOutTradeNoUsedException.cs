using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.Pay.WxException
{
    /// <summary>
    /// 微信单状态异常，订单下单异常异常：商户订单号重复
    /// </summary>
    public class WxOutTradeNoUsedException : ApplicationException
    {
        public WxOutTradeNoUsedException() { }
        public WxOutTradeNoUsedException(string message)
            : base(message) { }
        public WxOutTradeNoUsedException(string message, Exception inner)
            : base(message, inner) { }
    }
}