using System.Web.Mvc;
using EDTESP.Domain.Entities;
using EDTESP.Web.Helpers;

namespace EDTESP.Web.Filters
{
    public class CommomActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var usuario = filterContext.HttpContext.Session["Usuario"] as Usuario;
            filterContext.Controller.ViewBag.Usuario = usuario;

            var rbattrs = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(RibbonTabAttribute),
                false);

            if (rbattrs.Length == 1)
                filterContext.Controller.ViewBag.CurrentTab = ((RibbonTabAttribute)rbattrs[0]).Tab;

            base.OnActionExecuting(filterContext);
        }
    }
}