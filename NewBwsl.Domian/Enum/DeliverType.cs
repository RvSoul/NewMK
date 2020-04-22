using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.Enum
{
    /// <summary>
    /// 收货方式
    /// </summary>
    public enum DeliverType
    {
        /// <summary>
        /// 快递上门
        /// </summary>
        [Description("快递上门")]
        快递上门 = 0,
        /// <summary>
        /// 并单
        /// </summary>
        [Description("并单")]
        并单 = 1,
        /// <summary>
        /// 自提
        /// </summary>
        [Description("自提")]
        自提 = 2
    }
}