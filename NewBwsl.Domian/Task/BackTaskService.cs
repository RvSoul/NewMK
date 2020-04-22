using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Quartz;
using Quartz.Impl;

namespace NewMK.Domian.Task
{
    /// <summary>
    /// 后台任务调度BLL
    /// </summary>
    /// <remarks>
    ///   Author      :Wu ChangHong
    ///   Create date  :2019/10/25
    /// </remarks>
    public class BackTaskService
    {
        protected static log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// SchedulerFactory接口私有变量
        /// </summary>
        /// <remarks>
        /// <para>  Wu Changhong(2019/10/25)</para>
        /// </remarks>
        private ISchedulerFactory schedulerFactory;

        /// <summary>
        /// Scheduler接口私有变量
        /// </summary>
        /// <remarks>
        /// <para>  Wu Changhong(2019/10/25)</para>
        /// </remarks>
        private IScheduler scheduler;

        /// <summary>
        /// 私有构造函数
        /// </summary>
        /// <remarks>
        /// <para>  Wu Changhong(2019/10/25)</para>
        /// </remarks>
        public BackTaskService()
        {
            NameValueCollection properties = new NameValueCollection();
            properties["quartz.scheduler.instanceName"] = "XmlConfiguredInstance";
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "10";
            properties["quartz.threadPool.threadPriority"] = "Normal";

            // job initialization plugin handles our xml reading, without it defaults are used
            properties["quartz.plugin.xml.type"] = "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz";
            properties["quartz.plugin.xml.fileNames"] = "~/quartz_jobs.xml";
            schedulerFactory = new StdSchedulerFactory(properties);
            //scheduler = schedulerFactory.GetScheduler();
        }

        public string RootPath { get; set; }

        /// <summary>
        /// 启动任务
        /// </summary>
        /// <remarks>
        /// <para>  Wu Changhong(2019/10/25)</para>
        /// </remarks>
        public void OnStart(string rootPath)
        {
            if (scheduler != null)
                return;

            try
            {
                RootPath = rootPath;
                log.Info("站点物理路径：" + RootPath);
                if (scheduler == null)
                {
                    scheduler = schedulerFactory.GetScheduler();
                }
                scheduler.Start();
                log.Info("后台任务启动");
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        /// <summary>
        /// 停止任务
        /// </summary>
        /// <remarks>
        /// <para>  Wu Changhong(2019/10/25)</para>
        /// </remarks>
        public void OnStop()
        {
            try
            {
                if (scheduler != null)
                    scheduler.Shutdown(true);
                log.Info("后台任务停止");
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <remarks>
        /// <para>  Wu Changhong(2019/10/25)</para>
        /// </remarks>
        public void OnPause()
        {
            try
            {
                if (scheduler != null)
                    scheduler.PauseAll();
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        /// <summary>
        /// 继续任务
        /// </summary>
        /// <remarks>
        /// <para>  Wu Changhong(2019/10/25)</para>
        /// </remarks>
        public void OnContinue()
        {
            try
            {
                if (scheduler != null)
                    scheduler.ResumeAll();
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }
    }
}
