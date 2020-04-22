using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace NewMK.Domian.Task
{
    public abstract class BaseJob: IJob
    {
        protected static log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Job的实现
        /// </summary>
        /// <param name="context">Job执行的Context</param>
        /// <remarks>
        /// <para> 2019/10/25</para>
        /// </remarks>
        public void Execute(IJobExecutionContext context)
        {
            ExecuteJob();
        }

        /// <summary>
        /// 调用具体的Job实现
        /// </summary>
        protected abstract void ExecuteJob();
    }
}