using System.Net;
using System.Web.Mvc;
using EDTESP.Domain.Entities;

namespace EDTESP.Web.Helpers
{
    public class EdtespAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if(filterContext.Result is HttpUnauthorizedResult && string.IsNullOrEmpty(filterContext.HttpContext.User.Identity.Name))
                filterContext.Result = new RedirectResult("~/login");

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Controller.TempData["Usuario"] = filterContext.HttpContext.Session["Usuario"] as Usuario;
                filterContext.Result = new RedirectResult("~/home/naoautorizado");
            }
        }
    }
}