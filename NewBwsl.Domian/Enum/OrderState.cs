using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.Enum
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        /// 待付款
        /// </summary>
        [Description("待付款")]
        待付款 = 1,
        /// <summary>
        /// 待确认[废]
        /// </summary>
        [Description("待确认[废]")]
        待确认 = 2,
        /// <summary>
        /// 待发货
        /// </summary>
        [Description("待发货")]
        待发货 = 3,
        /// <summary>
        /// 订单完成，即订单支付完成
        /// </summary>
        [Description("订单完成")]
        订单完成 = 4,
        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款")]
        已退款 = 5,
        /// <summary>
        /// 支付已取消，即已删除
        /// </summary>
        [Description("支付已取消")]
        支付已取消 = 7,
        /// <summary>
        /// 支付失败
        /// </summary>
        [Description("支付失败")]
        支付失败 = 8,
        /// <summary>
        /// 待退款
        /// </summary>
        [Description("待退款")]
        待退款 = 9,
        /// <summary>
        /// 退款失败
        /// </summary>
        [Description("退款失败")]
        退款失败 = 10,
    }
}
