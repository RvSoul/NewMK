using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.Enum
{
    /// <summary>
    /// 会员等级
    /// </summary>
    public enum DeLevel
    {
        /// <summary>
        /// 游客
        /// </summary>
        [Description("游客")]
        游客 = 1,
        /// <summary>
        /// 顾客
        /// </summary>
        [Description("顾客")]
        顾客 = 2,
        /// <summary>
        /// VIP顾客
        /// </summary>
        [Description("VIP顾客")]
        VIP顾客 = 3,
        /// <summary>
        /// 创客
        /// </summary>
        [Description("创客")]
        创客 = 4
    }
}
