using MvcCMS.App_Start;
using MvcCMS.Models;
using MvcCMS.Models.ModelBinders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcCMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected async void Application_Start()
        {
           
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AreaRegistration.RegisterAllAreas();
            await AuthDbConfig.RegisterAdminAsync();

            ModelBinders.Binders.Add(typeof(Post),new PostModelBinder());
        }
    }
}
