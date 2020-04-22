using NewMK.DTO.Activity;
using NewMK.DTO.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.ShoppingCart
{
    public class ShoppingCartTT
    {
        public System.Guid ID { get; set; }
        public System.Guid ProductID { get; set; }
        public System.Guid UserID { get; set; }
        public int Num { get; set; }
        public Nullable<System.DateTime> AddTime { get; set; }
        public Nullable<int> ShoppingProductType { get; set; }

    }

    //public class ShoppingCartTTDTO
    //{
    //    public System.Guid ID { get; set; }
    //    public System.Guid ProductID { get; set; }
    //    public System.Guid UserID { get; set; }
    //    public int Num { get; set; }
    //    public Nullable<System.DateTime> AddTime { get; set; }
    //    public Nullable<int> ShoppingProductType { get; set; }          
    //    public decimal? ProductMoney { get; set; } 
    //}
    public class ShoppingCartDTO
    {
        public Guid? ID { get; set; }
        public Guid ProductID { get; set; }
        public Guid UserID { get; set; }
        public int Num { get; set; }
        public Nullable<System.DateTime> AddTime { get; set; }
        public Nullable<int> ShoppingProductType { get; set; }
        public Nullable<bool> IsXz { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> ShoppingProductPrice { get; set; }
        public Nullable<decimal> ShoppingProductPV { get; set; }
        public string zt { get; set; }
        public bool? IsBy { get; set; }
        public bool? IsYx { get; set; }
        public Nullable<decimal> PriceMin { get; set; }
        public Nullable<bool> IfCount { get; set; }
    }
 
    public class ShoppingCartActivityDTO
    {
        /// <summary>
        /// 购物车产品信息表
        /// </summary>
        public List<ShoppingCartDTO> sclist { get; set; }

        /// <summary>
        /// 已够活动产品信息表
        /// </summary>
        public List<ActivityProductPurchasedTT> applist { get; set; }

        /// <summary>
        /// 已赠活动产品信息表
        /// </summary>
        public List<ActivityGiftsPurchasedTT> agplist { get; set; }
    }

    public class ShoppingOrder
    {
        /// <summary>
        /// 购物数据集合json
        /// </summary>
        public string sclist { get; set; }

        /// <summary>
        /// 收货方式(0:送货上门 1:并单，2.自提)
        /// </summary>
        public int DeliverType { get; set; }

        /// <summary>
        /// 收获人
        /// </summary>
        public string ConsigneeName { get; set; }

        /// <summary>
        /// 地址电话
        /// </summary>
        public string ConsigneePhone { get; set; }

        /// <summary>
        /// 收货省
        /// </summary>
        public string ConsigneeProvince { get; set; }
        /// <summary>
        /// 收货市
        /// </summary>
        public string ConsigneeCity { get; set; }
        /// <summary>
        /// 收货县
        /// </summary>
        public string ConsigneeCounty { get; set; }

        /// <summary>
        /// 收获地址
        /// </summary>
        public string AddressInfo { get; set; }

        /// <summary>
        /// 所属人ID
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// 订单扣款人
        /// </summary>
        public Guid DealerId { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public int OrderTypeID { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        public Guid? ServiceCenterID { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        public int? DeLevelID { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public int PayType { get; set; }

        /// <summary>
        /// 推荐人
        /// </summary>
        public string PCDealerCode { get; set; }

        /// <summary>
        /// 安置人
        /// </summary>
        public string PPDealerCode { get; set; }

        /// <summary>
        /// 安置区域
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal? MoneyTransport { get; set; }
    }
}
