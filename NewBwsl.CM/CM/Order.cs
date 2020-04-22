//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewMK.Model.CM
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.ActivityGiftsPurchased = new HashSet<ActivityGiftsPurchased>();
            this.ActivityProductPurchased = new HashSet<ActivityProductPurchased>();
            this.OrderProduct = new HashSet<OrderProduct>();
        }
    
        public System.Guid ID { get; set; }
        public System.Guid UserID { get; set; }
        public System.Guid DealerId { get; set; }
        public int OrderTypeID { get; set; }
        public Nullable<System.Guid> ServiceCenterID { get; set; }
        public string OrderNumber { get; set; }
        public Nullable<decimal> MoneyProduct { get; set; }
        public Nullable<decimal> MoneyTransport { get; set; }
        public decimal OrderMoney { get; set; }
        public System.DateTime AddTime { get; set; }
        public int State { get; set; }
        public string ConsigneeName { get; set; }
        public string ConsigneePhone { get; set; }
        public string ConsigneeAddress { get; set; }
        public string LogisticsName { get; set; }
        public string LogisticsNum { get; set; }
        public Nullable<int> LogisticsState { get; set; }
        public Nullable<int> DeliverType { get; set; }
        public bool IsAppraise { get; set; }
        public Nullable<int> PayType { get; set; }
        public string IsBak { get; set; }
        public Nullable<decimal> OtherPayMoney { get; set; }
        public Nullable<decimal> UserPayMoney { get; set; }
        public Nullable<decimal> UserCouponsPay { get; set; }
        public decimal TotalPv { get; set; }
        public int IsBalance { get; set; }
        public Nullable<int> BalanceNumber { get; set; }
        public Nullable<int> DeLevelID { get; set; }
        public Nullable<int> OldDeLevelID { get; set; }
        public string PCDealerCode { get; set; }
        public string PPDealerCode { get; set; }
        public string DeptName { get; set; }
        public string ConsigneeProvince { get; set; }
        public string ConsigneeCity { get; set; }
        public string ConsigneeCounty { get; set; }
        public string RefundOrderNumber { get; set; }
        public Nullable<System.DateTime> RefundStartTime { get; set; }
        public Nullable<System.DateTime> RefundEndTime { get; set; }
        public byte SettleFlag { get; set; }
        public Nullable<System.DateTime> SettleTime { get; set; }
        public Nullable<System.DateTime> NextWxSynTime { get; set; }
        public string RefundRecvAccout { get; set; }
        public string ManualRefundComment { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityGiftsPurchased> ActivityGiftsPurchased { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityProductPurchased> ActivityProductPurchased { get; set; }
        public virtual OrderType OrderType { get; set; }
        public virtual ServiceCenter ServiceCenter { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderProduct> OrderProduct { get; set; }
    }
}
