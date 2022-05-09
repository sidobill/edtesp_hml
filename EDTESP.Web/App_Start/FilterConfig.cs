using System.Web;
using System.Web.Mvc;
using EDTESP.Web.Filters;

namespace EDTESP.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CommomActionFilter());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new FileDownloadAttribute());
        }
    }
}
