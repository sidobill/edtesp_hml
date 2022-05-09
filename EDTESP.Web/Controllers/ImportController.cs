using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace EDTESP.Web.Controllers
{
    public class ImportController : Controller
    {
        public ActionResult ImportCustomer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportCustomer(HttpPostedFileBase file)
        {
            
            return View();
        }
    }
}