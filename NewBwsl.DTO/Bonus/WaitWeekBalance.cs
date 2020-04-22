using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO.Bonus
{
    public class WaitWeekBalance
    {
        /// <summary>
        /// 结算期
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public string AdminName { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// 算奖截至日期
        /// </summary>
        public string CountDate { get; set; }
        /// <summary>
        /// 是否还在计算中
        /// </summary>
        public bool Working { get; set; }
        /// <summary>
        /// 总奖励积分
        /// </summary>
        public decimal Total { get; set; }
    }

    public class WeekPrizeHSDTO
    {
        /// <summary>
        /// 结算期
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// 结算日期  yyyy-MM-dd
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// 顾客编号
        /// </summary>
        public string DealerCode { get; set; }
        /// <summary>
        /// 顾客姓名
        /// </summary>
        public string DealerName { get; set; }
        /// <summary>
        /// 顾客证件号码
        /// </summary>
        public string DealerCardCode { get; set; }
        /// <summary>
        /// VIP收入
        /// </summary>
        public decimal Vip { get; set; }
        /// <summary>
        /// 销售奖
        /// </summary>
        public decimal Sale { get; set; }
        /// <summary>
        /// 直推奖
        /// </summary>
        public decimal Recom { get; set; }
        /// <summary>
        /// 店补级差
        /// </summary>
        public decimal Surplus { get; set; }
        /// <summary>
        /// 首货单推店奖
        /// </summary>
        public decimal RecomFirst { get; set; }
        /// <summary>
        /// 二货单推店奖
        /// </summary>
        public decimal RecomSecond { get; set; }

        /// <summary>
        /// 培育奖
        /// </summary>
        public decimal Teach { get; set; }
        /// <summary>
        /// 消费奖
        /// </summary>
        public decimal Record { get; set; }
        /// <summary>
        /// 店补
        /// </summary>
        public decimal ServiceCenter { get; set; }
        public decimal TimesSpendPrize { get; set; }
        /// <summary>
        /// 扣除消费 0.05
        /// </summary>
        public decimal ReduceSale { get; set; }
        /// <summary>
        /// 扣除服务费  0.07
        /// </summary>
        public decimal ReduceService { get; set; }

        /// <summary>
        /// 扣除信息费
        /// </summary>
        public decimal ReduceIT { get; set; }
        /// <summary>
        /// 实际应发奖励积分
        /// </summary>
        public decimal TotalPrize { get; set; }

        /// <summary>
        /// 计算次数
        /// </summary>
        public int PrizeCount { get; set; }

        /// <summary>
        /// 算奖时顾客的会员级别
        /// </summary>
        public string DeLevelName { get; set; }
        /// <summary>
        /// 算奖时顾客的荣誉级别
        /// </summary>
        public string HonLevelName { get; set; }

        /// <summary>
        /// 应发奖励积分
        /// </summary>
        public decimal yfjj { get; set; }

        public bool isGrant { get; set; }
        /// <summary>
        /// 发放日期
        /// </summary>
        public DateTime GrantDate { get; set; }
    }
    public class WeekPrizeDTO
    {
        /// <summary>
        /// 结算期
        /// </summary>
        public int? Num { get; set; }
        /// <summary>
        /// 结算日期  yyyy-MM-dd
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// 顾客编号
        /// </summary>
        public string DealerCode { get; set; }
        /// <summary>
        /// 顾客姓名
        /// </summary>
        public string DealerName { get; set; }
        /// <summary>
        /// 顾客证件号码
        /// </summary>
        public string DealerCardCode { get; set; }
        /// <summary>
        /// VIP收入
        /// </summary>
        public decimal? Vip { get; set; }
        /// <summary>
        /// 销售奖
        /// </summary>
        public decimal? Sale { get; set; }
        /// <summary>
        /// 直推奖
        /// </summary>
        public decimal? Recom { get; set; }
        /// <summary>
        /// 店补级差
        /// </summary>
        public decimal? Surplus { get; set; }
        /// <summary>
        /// 首货单推店奖
        /// </summary>
        public decimal? RecomFirst { get; set; }
        /// <summary>
        /// 二货单推店奖
        /// </summary>
        public decimal? RecomSecond { get; set; }
        /// <summary>
        /// 重复消费奖
        /// </summary>
        public decimal? TimesSpendPrize { get; set; }
        /// <summary>
        /// 培育奖
        /// </summary>
        public decimal? Teach { get; set; }
        /// <summary>
        /// 消费奖
        /// </summary>
        public decimal? Record { get; set; }
        /// <summary>
        /// 店补
        /// </summary>
        public decimal? ServiceCenter { get; set; }
        /// <summary>
        /// 扣除消费 0.05
        /// </summary>
        public decimal? ReduceSale { get; set; }
        /// <summary>
        /// 扣除服务费  0.07
        /// </summary>
        public decimal? ReduceService { get; set; }

        /// <summary>
        /// 扣除信息费
        /// </summary>
        public decimal? ReduceIT { get; set; }
        /// <summary>
        /// 实际应发奖励积分
        /// </summary>
        public decimal? TotalPrize { get; set; }

        /// <summary>
        /// 计算次数
        /// </summary>
        public int PrizeCount { get; set; }

        /// <summary>
        /// 算奖时顾客的会员级别
        /// </summary>
        public string DeLevelName { get; set; }
        /// <summary>
        /// 算奖时顾客的荣誉级别
        /// </summary>
        public string HonLevelName { get; set; }
        /// <summary>
        /// 发放日期
        /// </summary>
        public DateTime GrantDate { get; set; }
    }

}
