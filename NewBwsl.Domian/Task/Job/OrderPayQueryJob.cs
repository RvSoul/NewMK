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
    /// 订单支付查询Job
    /// </summary>
    public class OrderPayQueryJob : BaseJob
    {
        protected static log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 同步处理的变量
        /// </summary>
        private static readonly object sysObject = new object();

        /// <summary>
        /// 同步确认的时间间隔控制，单位：秒
        /// </summary>
        private static readonly int[] TS = new int[5] { 30, 50, 60, 120,600 };

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
            var entitys = orderDM.FindNoPay(10);
            log.Debug($"获取到{entitys.Count}条待与微信财务通确认的待付款订单！");
            foreach (var entity in entitys)
            {
                try
                {
                    log.Info($"订单[{entity.OrderNumber}]，同步微信财务通的支付状态开始！");
                    orderDM.OrderConfirmByWx(entity.OrderNumber);
                    log.Info($"订单[{entity.OrderNumber}]，同步微信财务通的支付状态结束！");
                }
                catch (WxOrderNotPayException)
                {
                    var dt = GetNextSyncTime(entity.AddTime);
                    log.Info($"订单[{entity.OrderNumber}]，调度到[{dt.ToString("yyyy-MM-dd HH:mm:ss:ffff")}]后同步微信支付状态！");
                    //对订单做延时处理
                    orderDM.ChangeWxSynTime(dt, entity.ID);
                }
                catch(WxOrderNotExistsException)
                {
                    orderDM.PayCancel(entity.OrderNumber);
                    log.Info($"订单[{entity.OrderNumber}]已取消！");
                }
                catch (Exception ex)
                {
                    log.Info($"订单号[{entity.OrderNumber}]，{ex.Message}");
                }
            }
        }

        private DateTime GetNextSyncTime(DateTime addTime)
        {
            var ts = (DateTime.Now - addTime).TotalSeconds;
            var totalSecond = 0;
            foreach(int t in TS)
            {
                totalSecond += t;
                if(ts < totalSecond)
                {
                    return DateTime.Now.AddSeconds(t);
                }
            }
            
            return DateTime.Now.AddSeconds(TS[TS.Length - 1]);
        }
    }
}