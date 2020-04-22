using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NewMK.Domian.DM;
using NewMK.Domian.DomainException;
using NewMK.Domian.Pay.WxException;
using Quartz;

namespace NewMK.Domian.Task.Job
{
    /// <summary>
    /// 安置订单Job
    /// </summary>
    public class OrderBalanceAccountJob : BaseJob
    {
        protected static log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 同步处理的变量
        /// </summary>
        private static readonly object sysObject = new object();

        /// <summary>
        /// 是否正在执行
        /// </summary>
        private static bool isDoing = false;

        OrderDM orderDM = new OrderDM();

        /// <summary>
        /// 活动报名Job
        /// </summary>
        protected override void ExecuteJob()
        {
            try
            {
                lock (sysObject)
                {
                    if (isDoing)
                        return;
                    isDoing = true;
                }
                try
                {
                    Execute();
                }
                finally
                {
                    isDoing = false;
                }
            }
            catch (Exception em)
            {
                isDoing = false;
                log.Error(em);
            }
        }
        private void Execute()
        {
            var entitys = orderDM.FindBalanceAccount(10);
            log.Debug($"获取到{entitys.Count}条待结算订单！");
            foreach (var entity in entitys)
            {
                try
                {
                    log.Info($"订单[{entity.OrderNumber}]安置开始！");
                    orderDM.BalanceAccount(entity.ID);
                    log.Info($"订单[{entity.OrderNumber}]安置结束！");
                }
                catch (Exception ex)
                {
                    log.Info($"订单[{entity.OrderNumber}]安置出错，错误信息：{ex.Message}", ex);
                }
            }
        }
    }
}