using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.Enum
{
    /// <summary>
    /// 订单类型
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 注册单
        /// </summary>
        [Description("注册单")]
        注册单 = 1,
        /// <summary>
        /// 升级单
        /// </summary>
        [Description("升级单")]
        升级单 = 2,
        /// <summary>
        /// 主动消费单
        /// </summary>
        [Description("主动消费单")]
        主动消费单 = 3,
        /// <summary>
        /// 抢购单
        /// </summary>
        [Description("抢购单")]
        抢购单 = 4
    }
}
