using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;

namespace EDTESP.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IAppServiceBase<Status>  appsvcStatus) : base(appsvcStatus)
        {
            
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NaoAutorizado()
        {
            return View("403");
        }
    }
}