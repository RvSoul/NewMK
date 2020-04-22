using NewMK.Domian.Task;
using Topshelf;
using Utility;

namespace BackTask.NewMK.Service.BackTask
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                //x.UseAssemblyInfoForServiceInfo();
                x.Service<BackTaskService>(s =>
                {
                    s.ConstructUsing(name => new BackTaskService());
                    s.WhenStarted(tc => tc.OnStart(ReadConfig.GetConsolePath()));
                    s.WhenStopped(tc => tc.OnStop());
                });
                x.SetServiceName("新零售系统服务");
                x.SetDisplayName("新零售系统服务服务");
                x.SetInstanceName("NewMK");
                x.SetDescription("负责处理后台数据分析和处理");
                x.RunAsLocalSystem();
                x.StartAutomatically();
            });
        }
    }
}
