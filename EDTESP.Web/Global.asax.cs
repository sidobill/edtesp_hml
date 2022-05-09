using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EDTESP.Application.Interfaces;
using Unity.Resolution;
using Microsoft.Practices.Unity;

namespace EDTESP.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            UnityConfig.RegisterComponents();
            AutoMapperConfig.Configure();
        }



        protected void Application_AuthenticateRequest(object sender, EventArgs args)
        {
            if (Context.User == null || string.IsNullOrEmpty(Context.User.Identity.Name)) return;

            var gp = new GenericPrincipal(Context.User.Identity, new string[]{});

            var login = Context.User.Identity.Name;

            var appsvc = (IUsuarioAppService)UnityConfig.Container.Resolve(typeof(IUsuarioAppService),"");
            var usr = appsvc.Where(x => !x.Removido && x.Login == login).FirstOrDefault();
            if (usr != null)
            {
                var prms = appsvc.ObterPermissoes(usr.UsuarioId);
                gp = new GenericPrincipal(Context.User.Identity, prms.Select(x => x.Role).ToArray());
            }

            Context.User = gp;
        }
    }
}
