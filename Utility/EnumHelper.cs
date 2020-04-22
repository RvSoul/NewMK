using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;

namespace Infrastructure.Utility
{
    /// <summary>
    /// 枚举处理Helper
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 获取枚举Description Attribute的值
        /// </summary>
        /// <param name="value">枚举</param>
        /// <returns></returns>
        public static string GetDescription(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            var field = value.GetType().GetField(value.ToString());


            if (field == null)
            {
                return string.Empty;
            }


            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;


            return attribute == null ? value.ToString() : attribute.Description;
        }


        /// <summary>
        /// 获取枚举Description Attribute的值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">枚举值</param>
        /// <returns></returns>
        public static string GetDescription<T>(int? value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            return GetDescription((T)System.Enum.Parse(typeof(T), value.Value.ToString()));
        }
    }
}