using NewMK.Domian.DomainException;
using NewMK.DTO.Bonus;
using NewMK.Model.CM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.Domian.DM
{
    public class BonusDM
    {
        int weektype = 0;
        public List<WaitWeekBalance> BalanceWeekPrize()
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                string sql = @"select a.BalanceNumber Num,a.CreatePerson AdminName,convert(nvarchar(20),a.CreateDate,102) [Date],isnull(convert(nvarchar(20),a.CountDate,102),'') [CountDate],a.IsWorking [Working],isnull(sum(b.TotalPrize),0) Total 
                        from [Balance] a left join [WeekPrize] b on a.BalanceNumber=b.BalanceNumber 
                        where a.IsGrant=1 and a.BalanceType=0 and a.TopBalanceNumber is null group by a.BalanceNumber,a.CreatePerson,a.CreateDate,a.IsWorking,a.CountDate order by Num desc";
                return c.Database.SqlQuery<WaitWeekBalance>(sql).ToList();
            }
        }
        string sqltext = "Data Source=172.168.2.31;Initial Catalog=BwslRetailRelation;User ID=SQLuserFL;pwd=hqG@5G8K42ahNsq";
        public WaitWeekBalance BalanceWeekPrize(string weekprizefilepath, DateTime date, string adminperson)
        {
            int maxbalancenumber = 0;

            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {

                int sfjs = cc.Database.ExecuteSqlCommand("  exec Pro_Check_PRelation_Balance  ");
                if (sfjs >= 1)
                {
                    throw new DMException("辅导关系有问题，请检查后结算！");
                }
                if (cc.Balance.Any(a => a.IsWorking))
                {
                    throw new DMException("存在奖励积分计算中，暂时无法计算周奖！");
                }
                if (cc.WeekPrize.Any(a => !a.IsGrant))
                {
                    throw new DMException("存在未发周奖，暂时无法计算周奖！");
                }
                if (cc.Balance.Where(m => m.BalanceType == weektype).Any())
                {
                    maxbalancenumber = cc.Balance.Where(m => m.BalanceType == weektype).Max(m => m.BalanceNumber) + 1;
                }
                else
                {
                    maxbalancenumber = 1;
                }


            }

            using (SqlConnection conn = new SqlConnection(sqltext))
            {
                conn.Open();
                //string sql1 = "SELECT max([BalanceNumber]) FROM [Balance]";
                //SqlCommand cmd1 = new SqlCommand(sql1, conn);
                //maxbalancenumber = Convert.ToInt32(cmd1.ExecuteScalar()) + 1;

                string sql2 = "Exec Pro_Count_WeekPrize '" + date.ToString() + "','" + weektype + "','" + maxbalancenumber + "','" + adminperson + "'";
                SqlCommand cmd2 = new SqlCommand(sql2, conn);
                cmd2.CommandTimeout = 0;

                cmd2.ExecuteNonQuery();

            }

            return new WaitWeekBalance()
            {
                AdminName = adminperson,
                Date = DateTime.Now.ToString("yyyy.MM.dd"),
                Num = maxbalancenumber,
                Total = 0,
                Working = true,
                CountDate = date.ToString("yyyy.MM.dd")
            };
        }

        public bool QueryBalance(int balance, int type)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                //using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
                {
                    if (c.Balance.Where(m => m.BalanceNumber == balance && m.BalanceType == type).Any())
                    {
                        return !c.Balance.Where(m => m.BalanceNumber == balance && m.BalanceType == type).FirstOrDefault().IsWorking;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        public List<WaitWeekBalance> GetWaitGrantWeekBalanceList()
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                string sql = @"select convert(nvarchar(20),a.CreateDate,102) [Date],isnull(convert(nvarchar(20),a.CountDate,102),'') [CountDate],a.CreatePerson [AdminName],a.BalanceNumber [Num],isnull(b.Total,0) Total,a.IsWorking [Working] from  [Balance] a 
                     left join (select BalanceNumber,sum(TotalPrize) Total from  [WeekPrize] where IsGrant=0 group by BalanceNumber) b on a.BalanceNumber=b.BalanceNumber 
                     where a.IsGrant=0 and a.BalanceType=0 and a.BalanceNumber not in (select distinct BalanceNumber from [WeekPrize]  where IsGrant=1) and a.TopBalanceNumber is null";
                return c.Database.SqlQuery<WaitWeekBalance>(sql).ToList();
            }
        }

        public string GrantPrize(string weekprizefilepath, int balancenumber, string adminperson)
        {

            using (BwslRetailRelationEntities cc = new BwslRetailRelationEntities(BwslRetailRelationEntities.GetConnection()))
            {
                int weektype = 0;
                var balance = cc.Balance.Where(m => m.BalanceNumber == balancenumber && m.BalanceType == weektype).FirstOrDefault();
                if (balance == null)
                {
                    throw new NullReferenceException("未找到匹配的周奖的计算记录！");
                }
                if (balance.IsGrant || cc.WeekPrize.Where(m => m.BalanceNumber == balancenumber && m.IsGrant).Any())
                {
                    throw new DMException("周奖已发放！");
                }
                if (balance.IsWorking)
                {
                    throw new DMException("周奖正在计算中无法发放！");
                }


            }

            using (SqlConnection conn = new SqlConnection(sqltext))
            {
                conn.Open();

                string sql2 = "Exec Pro_Grant_WeekPrize '" + balancenumber + "','" + adminperson + "'";
                SqlCommand cmd2 = new SqlCommand(sql2, conn);
                cmd2.CommandTimeout = 0;

                return cmd2.ExecuteScalar().ToString();

            }

        }

        public List<WeekPrizeHSDTO> GetWeekPrizeList(bool isgrant, int? num, DateTime? startdate, DateTime? enddate, string dealecode, string dealername, int page, int size, out int totalcount)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                string sql = string.Format(@"SELECT (row_number() over(order by a.BalanceNumber desc,a.TotalPrize)-1)/{7}+1 r,a.BalanceNumber [Num],
                a.DealerDeLevel [DeLevelName],a.DealerHonLevel [HonLevelName],convert(nvarchar(20),b.CreateDate,102) [Date],c.UserCode DealerCode,c.UserName DealerName,
                c.CardCode DealerCardCode,
                a.VipPrize [Vip],a.SalePrize [Sale],
                a.RecomPrize [Recom],
                a.ServiceSurplus [Surplus],
                a.RecomServiceFirst [RecomFirst],a.isGrant,
                a.RecomPrize+a.SalePrize+a.TeachPrize+a.RecordPrize+a.ServicePrize+a.ServiceSurplus+a.RecomServiceFirst+a.RecomServiceSecond +a.TimesSpendPrize as yfjj,
                a.RecomServiceSecond [RecomSecond],a.TeachPrize [Teach],a.RecordPrize [Record],a.ServicePrize [ServiceCenter],a.TimesSpendPrize,
                a.ReduceSale [ReduceSale],
                a.ReduceService [ReduceService],a.ReduceIT [ReduceIT],a.TotalPrize [TotalPrize] ,a.GrantDate FROM [WeekPrize] a 
                inner join  [Balance] b on a.BalanceNumber=b.BalanceNumber and b.BalanceType={4} and {1} and {6} and {5}
                inner join  [User] c on a.DealerId=c.Id  and {2} and {3}
                ",
                //where a.isGrant={0} 
                isgrant ? 1 : 0,
                startdate.HasValue ? ("b.CreateDate >= '" + ((DateTime)startdate).ToString("yyyy-MM-dd HH:mm:ss") + "'") : "1=1",
                !string.IsNullOrEmpty(dealecode) ? " c.UserCode=@p0" : "1=1",
                !string.IsNullOrEmpty(dealername) ? " c.UserName=@p1" : "1=1",
                weektype,
                num.HasValue ? "b.BalanceNumber=" + num.ToString() : "1=1",
                enddate.HasValue ? ("b.CreateDate <= '" + ((DateTime)enddate).ToString("yyyy-MM-dd HH:mm:ss") + "'") : "1=1",
                size);
                totalcount = c.Database.SqlQuery<WeekPrizeHSDTO>(sql, dealecode, dealername).Count();

                string pagesql = string.Format("select * from ({0}) a where a.r={1}", sql, page);
                return c.Database.SqlQuery<WeekPrizeHSDTO>(pagesql, dealecode, dealername).ToList();
            }
        }

        public List<WeekPrizeDTO> GetWeekPrizeDownload(bool isgrant, int? num, DateTime? startdate, DateTime? enddate, string dealecode, string dealername)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                string sql = string.Format(@"SELECT a.BalanceNumber [Num],a.DealerDeLevel [DeLevelName],a.DealerHonLevel [HonLevelName],convert(nvarchar(20),b.CreateDate,102) [Date],c.UserCode DealerCode,c.UserName DealerName,c.CardCode DealerCardCode,         
                a.TeachPrize [Teach],
                a.VipPrize [Vip],a.SalePrize [Sale],a.RecomPrize [Recom],a.ServiceSurplus [Surplus],a.RecomServiceFirst [RecomFirst],a.RecomServiceSecond [RecomSecond], a.RecordPrize [Record],a.ServicePrize [ServiceCenter],a.TimesSpendPrize,
                a.ReduceSale [ReduceSale],a.ReduceService [ReduceService],a.ReduceIT [ReduceIT],a.TotalPrize [TotalPrize] ,a.GrantDate  FROM [dbo].[WeekPrize] a 
                inner join [dbo].[Balance] b on a.BalanceNumber=b.BalanceNumber and b.BalanceType={0} and {1} and {2} and {3}
                inner join [dbo].[User] c on a.DealerId=c.Id  and {4} and {5}
                where {6} order by b.BalanceNumber,c.UserCode ",
                weektype,
                startdate.HasValue ? ("b.CreateDate >= '" + ((DateTime)startdate).ToString("yyyy-MM-dd HH:mm:ss") + "'") : "1=1",
                enddate.HasValue ? ("b.CreateDate <= '" + ((DateTime)enddate).ToString("yyyy-MM-dd HH:mm:ss") + "'") : "1=1",
                num.HasValue ? "b.BalanceNumber=" + num.ToString() : "1=1",
                !string.IsNullOrEmpty(dealecode) ? " c.UserCode=@p0" : "1=1",
                !string.IsNullOrEmpty(dealername) ? " c.UserName=@p1" : "1=1",
                "1=1");



                return c.Database.SqlQuery<WeekPrizeDTO>(sql, dealecode, dealername).ToList();
            }
        }
        public WeekPrizeDTO GetWeekPrizeTotal(bool isgrant, int? num, DateTime? startdate, DateTime? enddate, string dealercode, string dealername)
        {
            using (BwslRetailEntities c = new BwslRetailEntities())
            {
                string sql = string.Format(@"SELECT 0 [Num],'' [DeLevelName],'' [HonLevelName],'' [Date],'' DealerCode,'' DealerName,'' DealerCardCode,
                isnull(sum(a.VipPrize),0) [Vip],isnull(sum(a.SalePrize),0) [Sale],isnull(sum(a.RecomPrize),0) [Recom],
                isnull(sum(a.ServiceSurplus),0) [Surplus],
                isnull(sum(a.RecomServiceFirst),0) [RecomFirst],
                isnull(sum(a.RecomServiceSecond),0) [RecomSecond],isnull(sum(a.TeachPrize),0) [Teach],isnull(sum(a.RecordPrize),0) [Record],
                isnull(sum(a.ServicePrize),0) [ServiceCenter],Sum(IsNull(TimesSpendPrize,0)) TimesSpendPrize,
                isnull(sum(a.ReduceSale),0) [ReduceSale],isnull(sum(a.ReduceService),0) [ReduceService],isnull(sum(a.ReduceIT),0) [ReduceIT],
                isnull(sum(a.TotalPrize),0) [TotalPrize]  FROM [dbo].[WeekPrize] a 
                inner join [dbo].[Balance] b on a.BalanceNumber=b.BalanceNumber and b.BalanceType={4} and {1} and {6} and {5}
                inner join [dbo].[User] c on a.DealerId=c.Id and {2} and {3}
                where {0} ",
               //a.isGrant=    isgrant ? 1 : 0
               "1=1",
               startdate.HasValue ? ("b.CreateDate >= '" + ((DateTime)startdate).ToString("yyyy-MM-dd HH:mm:ss") + "'") : "1=1",
               !string.IsNullOrEmpty(dealercode) ? " c.UserCode=@p0" : "1=1",
               !string.IsNullOrEmpty(dealername) ? " c.UserName=@p1" : "1=1",
               weektype,
               num.HasValue ? "b.BalanceNumber=" + num.ToString() : "1=1",
               enddate.HasValue ? ("b.CreateDate <= '" + ((DateTime)enddate).ToString("yyyy-MM-dd HH:mm:ss") + "'") : "1=1");

                return c.Database.SqlQuery<WeekPrizeDTO>(sql, dealercode, dealername).FirstOrDefault();
            }
        }

    }
}
