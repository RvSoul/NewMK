using NewMK.DTO.User; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Order
{
    public class Request_Order
    {
        [SelectField("and", "in", "string")]
        public string OrderNumber { get; set; }

        
        public int? State { get; set; }

        [SelectField("and", "=", "int")]
        public int? OrderTypeID { get; set; }

        [SelectField("and", "in", "string")]
        public string ConsigneeAddress { get; set; }

        [SelectField("and", "in", "string")]
        public string ConsigneeName { get; set; }

        [SelectField("and", "in", "string")]
        public string ConsigneePhone { get; set; }

        [SelectField("and", "in", "string")]
        public string LogisticsNum { get; set; }
        [SelectField("and", "=", "int")]
        public int? IsBalance { get; set; }
        public bool? IsDG { get; set; }
        public bool? IsPay { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public Nullable<System.DateTime> STime { get; set; }
        public Nullable<System.DateTime> ETime { get; set; }
        public int? je1 { get; set; }
        public int? je2 { get; set; }
        public int? pv1 { get; set; }
        public int? pv2 { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }

    public class OrdersModel
    {
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
        public string ConsigneeProvince { get; set; }
        public string ConsigneeCity { get; set; }
        public string ConsigneeCounty { get; set; }
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
        public byte SettleFlag { get; set; }
        public string DeptName { get; set; }
        public string RefundOrderNumber { get; set; }
        public Nullable<System.DateTime> RefundStartTime { get; set; }
        public Nullable<System.DateTime> RefundEndTime { get; set; }
        public Nullable<System.DateTime> SettleTime { get; set; }
        public Nullable<System.DateTime> NextWxSynTime { get; set; }
        public string RefundRecvAccout { get; set; }
        public string ManualRefundComment { get; set; }
    }
    public class OrdersDTO : OrdersModel
    {
        public string OrderTypeName { get; set; }
        public string DeLevelName { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string DealerPhone { get; set; }
        public string ServiceCenterName { get; set; }
        public string ServiceCenterCode { get; set; }
        public string RefundOrderNumber { get; set; }
        public List<OrderProductDTOQD> OrderProduct { get; set; }

    }

    public class OrderStatrMoney
    {
        public Nullable<decimal> MoneyProduct { get; set; }
        public decimal TotalPv { get; set; }
    }

    public class PayOrderUserDTO
    {
        public int OrderTypeID { get; set; }
        public int DeLevelID { get; set; }
        public string Phone { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
    }
}
