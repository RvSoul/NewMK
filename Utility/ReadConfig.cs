using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.Web;

namespace Utility
{
    public class ReadConfig
    {
        /// <summary>
        /// 读取配置文件中的AppSetting节点值
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static string GetAppSetting(string appName)
        {
           return ConfigurationManager.AppSettings[appName];
        }

        /// <summary>
        /// 读取配置文件中的ConnectionString节点值
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public static string GetConnectionString(string connectionName)
        {
            return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }

        public static string GetAppPath()
        {
            return HttpRuntime.AppDomainAppPath;
        }

        public static string GetConsolePath()
        {
            return Environment.CurrentDirectory;
        }
    }
}
