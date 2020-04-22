using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.AdminUser
{
    public class FreightRulesDTO
    {
        /// <summary>
        /// 页大小
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { get; set; }


        public System.Guid ID { get; set; }
        public Nullable<int> ProvinceID { get; set; }
        public string ProvinceName { get; set; }
        public Nullable<decimal> Price1 { get; set; }
        public Nullable<decimal> Price2 { get; set; }
        public Nullable<decimal> Price3 { get; set; }
        public Nullable<decimal> Price4 { get; set; }
    }
}
