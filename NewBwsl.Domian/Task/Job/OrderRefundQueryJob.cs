using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NewMK.Domian.DM;
using NewMK.Domian.DomainException;
using NewMK.Domian.Pay.WxException;
using Quartz;
using WxPayAPI;
using DomainEnum = NewMK.Domian.Enum;

namespace NewMK.Domian.Task.Job
{
    /// <summary>
    /// 订单退款查询Job
    /// </summary>
    public class OrderRefundQueryJob : BaseJob
    {
        protected static log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 同步处理的变量
        /// </summary>
        private static readonly object sysObject = new object();

        /// <summary>
        /// 同步确认的时间间隔控制，单位：分钟
        /// </summary>
        private static readonly int[] TS = new int[5] { 3, 5, 10, 30,60 };

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
            var entitys = orderDM.FindRefundDoing(10);
            log.Debug($"获取到{entitys.Count}条待与微信财务通确认的待退款订单！");
            foreach (var entity in entitys)
            {
                log.Info($"订单[{entity.OrderNumber}]，同步微信财务通的退款状态开始！");
                WxPayData req = new WxPayData();
                req.OutTradeNo = entity.OrderNumber;
                var notifyData = WxPayApi.RefundQuery(req);
                var refundStatus = notifyData.GetValue("refund_status_0");
                if (refundStatus.Equals(WxPayApi.REFUNDQUERY_STATUS_SUCCESS))
                {
                    orderDM.RefundSuccess(entity.OrderNumber, notifyData.GetStringValue("refund_recv_accout_0"));
                }
                else if (refundStatus.Equals(WxPayApi.REFUNDQUERY_STATUS_PROCESSING))
                {
                    var dt = GetNextSyncTime(entity.AddTime);
                    log.Info($"订单[{entity.OrderNumber}]，调度到[{dt.ToString("yyyy-MM-dd HH:mm:ss:ffff")}]后同步微信退款状态！");
                    //对订单做延时处理
                    orderDM.ChangeWxSynTime(dt, entity.ID);
                }
                else
                {
                    orderDM.RefundFail(entity.OrderNumber);
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
                    return DateTime.Now.AddMinutes(t);
                }
            }
            
            return DateTime.Now.AddSeconds(TS[TS.Length - 1]);
        }
    }
}