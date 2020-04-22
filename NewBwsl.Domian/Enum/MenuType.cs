using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.Enum
{
    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum MenuType
    {
        /// <summary>
        /// 菜单
        /// </summary>
        [Description("菜单")]
        Menu = 0,
        /// <summary>
        /// 菜单行为
        /// </summary>
        [Description("菜单行为")]
        MenuAction = 1
    }
}
