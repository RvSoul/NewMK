using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.Enum
{
    /// <summary>
    /// 结算标志
    /// </summary>
    public enum SettleFlag
    {
        /// <summary>
        /// 不参与结算
        /// </summary>
        [Description("不参与结算")]
        不参与结算 = 0,
        /// <summary>
        /// [支付]人工处理后结算
        /// </summary>
        [Description("人工处理后结算")]
        人工处理后结算 = 1,
        /// <summary>
        /// [支付]待结算
        /// </summary>
        [Description("待结算")]
        待结算 = 2,
        /// <summary>
        /// [支付]结算完成
        /// </summary>
        [Description("结算完成")]
        结算完成 = 3,

        /// <summary>
        /// [支付]结算失败
        /// </summary>
        [Description("结算失败")]
        结算失败 = 4
    }
}