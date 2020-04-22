using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.User
{ 
    /// <summary>
    /// 推荐关系顾客
    /// </summary>
    public class DealerRelationDTO
    {
        /// <summary>
        /// 顾客编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 顾客姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 顾客会员级别
        /// </summary>
        public string Delevel { get; set; }
        /// <summary>
        /// 顾客荣誉级别
        /// </summary>
        public string Honlevel { get; set; }
        /// <summary>
        /// 安置区域
        /// </summary>
        public string AreaName { get; set; }
    }

    public class CKRelationDTO
    {
        /// <summary>
        /// 安置
        /// </summary>
        public bool PR { get; set; }
        /// <summary>
        /// 推荐
        /// </summary>
        public bool RR { get; set; }
    }
}
