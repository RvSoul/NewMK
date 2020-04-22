using NewMK.DTO.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Product
{

    public class Request_ProductDTO : ModelDTO
    {
        [SelectField("and", "=", "string")]
        public string ProductCode { get; set; }

        [SelectField("and", "=", "Guid")]
        public Guid? ProductTypeID { get; set; }

        [SelectField("and", "in", "string")]
        public string ProductName { get; set; }

        [SelectField("and", "=", "bool")]
        public bool? State { get; set; }

        [SelectField("and", "in", "string")]
        public string KeyWord { get; set; }

        [SelectField("and", "=", "bool")]
        public bool? IsBy { get; set; }

        public bool? IsStarProduct { get; set; }

        public int? OrderTypeID { get; set; }
        public string LevelList { get; set; }
        public string OldLevelList { get; set; }
    }

    public partial class ProductModel
    {

        public System.Guid ID { get; set; }
        public System.Guid ProductTypeID { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public System.DateTime AddTime { get; set; }
        public string SalesVolume { get; set; }
        public string KeyWord { get; set; }
        public string ProductWeight { get; set; }
        public string Unit { get; set; }
        public decimal PV { get; set; }
        public decimal Price { get; set; }
        public bool State { get; set; }
        public int ProductNature { get; set; }
        public string CrProductId { get; set; }
        public Nullable<int> PX { get; set; }
        public Nullable<int> Mulriple { get; set; }
        public bool IsBy { get; set; }
        public Nullable<System.DateTime> STime { get; set; }
        public Nullable<System.DateTime> ETime { get; set; }
        public int StockNumber { get; set; }
        public Nullable<bool> IsStarProduct { get; set; }

    }
    public class ProductDTO : ProductModel
    {
        /// <summary>
        /// 产品类型名称
        /// </summary>
        public string ProductTypeName { get; set; }

        /// <summary>
        /// 产品数量
        /// </summary>
        public int ProductNum { get; set; }

        /// <summary>
        /// 主图
        /// </summary>
        public string zt { get; set; }

        public List<ActivityDTO> ActivityList { get; set; }

    }
}
