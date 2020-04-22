using NewMK.Domian.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Manage.NewMK.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        //BackTaskService backTaskService = new BackTaskService();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //backTaskService.OnStart(Environment.CurrentDirectory);
        }

        void Application_End(object sender, EventArgs e)
        {
            //backTaskService.OnStop();
        }
    }
}
