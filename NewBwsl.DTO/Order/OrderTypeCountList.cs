using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Order
{
    /// <summary>
    /// 订单类型统计
    /// </summary>
    public class OrderTypeCountList
    {
        public int CountTT { get; set; }
        public string OrderTypeName { get; set; }
        public decimal? MoneyProduct { get; set; }
        public decimal? MoneyTransport { get; set; }
        public decimal? OrderMoney { get; set; }
        public decimal TotalPv { get; set; }
    }

    public class Pro_GetSaleTeam_OrderDTO
    {

        public DateTime AddTime { get; set; }
        public string OrderNumber { get; set; }
        public string OrderTypeName { get; set; }
        public decimal MoneyProduct { get; set; }
        public decimal MoneyTransport { get; set; }
        public decimal OtherPayMoney { get; set; }
        public decimal UserPayMoney { get; set; }
        public string State { get; set; }
        public string IsBalance { get; set; }
    }
}
