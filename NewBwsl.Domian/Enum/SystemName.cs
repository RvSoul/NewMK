using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.Enum
{
    /// <summary>
    /// 系统归属
    /// </summary>
    public enum SystemName
    {
        /// <summary>
        /// 后台系统
        /// </summary>
        [Description("后台系统")]
        后台系统 = 1,

        /// <summary>
        /// 核算系统
        /// </summary>
        [Description("核算系统")]
        核算系统 = 2
    }
}