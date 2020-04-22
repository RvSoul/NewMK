using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Record
{
    public class ServiceCenterPrizeInfoDTO
    {

        /// <summary>
        /// 结算期
        /// </summary>
        public int BalanceNumber { get; set; }
        /// <summary>
        /// 店铺类型
        /// </summary>
        public string SCType { get; set; }
        /// <summary>
        /// 顾客编号
        /// </summary>
        public string DealerCode { get; set; }

        /// <summary>
        /// 顾客姓名
        /// </summary>
        public string DealerName { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 当期升级，注册新增PV
        /// </summary>
        public int TotalPV { get; set; }
        /// <summary>
        /// 奖金计算比例
        /// </summary>
        public decimal Percentum { get; set; }

    }
    /// <summary>
    /// 主动消费奖详情
    /// </summary>
    public class RecordPrizeInfoDTO
    {
        /// <summary>
        /// 顾客编号
        /// </summary>
        public string DealerCode { get; set; }

        /// <summary>
        /// 顾客姓名
        /// </summary>
        public string DealerName { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int Layer { get; set; }

        /// <summary>
        /// 当期消费单PV
        /// </summary>
        public int TotalPV { get; set; }

        /// <summary>
        /// 奖金计算比例
        /// </summary>
        public decimal Percentum { get; set; }
    }
}
