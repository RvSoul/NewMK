using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.Enum
{
    /// <summary>
    /// 支付渠道
    /// </summary>
    public enum PayType
    {
        /// <summary>
        /// 积分
        /// </summary>
        [Description("积分")]
        积分 = 1,
        /// <summary>
        /// 微信
        /// </summary>
        [Description("微信")]
        微信 = 2,
        /// <summary>
        /// 快钱
        /// </summary>
        [Description("快钱")]
        快钱 = 3,
        /// <summary>
        /// 快钱
        /// </summary>
        [Description("微信加积分")]
        微信加积分 = 4,
        /// <summary>
        /// 快钱
        /// </summary>
        [Description("快钱加积分")]
        快钱加积分 = 5,
    }
}