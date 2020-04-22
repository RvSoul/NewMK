using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Product
{
    public class GiftProductTT
    {
        public System.Guid ID { get; set; }
        public System.Guid ProductID { get; set; }
        public System.Guid GiftID { get; set; }
        public int GiftNum { get; set; }
        /// <summary>
        /// 用户类型，PS：单个：1；多个：1-2-3（多个中间用-隔开）
        /// </summary>
        public string DeLevelID { get; set; }
        public decimal GiftPrice { get; set; }
    }
    public class GiftProductDTO
    {
        public System.Guid ID { get; set; }
        public System.Guid ProductID { get; set; }
        public System.Guid GiftID { get; set; }
        public int GiftNum { get; set; }
        public string DeLevelID { get; set; }
        public decimal GiftPrice { get; set; }

        public string ProductName { get; set; }
        public string ProductCode { get; set; }
    }
}
