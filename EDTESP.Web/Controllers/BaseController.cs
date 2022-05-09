using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Infrastructure.CC.Util;
using EDTESP.Web.Helpers;
using Newtonsoft.Json;
using PagedList;

namespace EDTESP.Web.Controllers
{
    [EdtespAuthorize]
    public class BaseController : Controller
    {
        private const string FileDownloadCookieName = "fileDownload";

        protected Usuario Usuario;

        protected IAppServiceBase<Status> _appsvcStatus;

        private void CheckAndHandleFileResult(ResultExecutingContext context)
        {
            if (context.Result is FileResult)
                //jquery.fileDownload uses this cookie to determine that a file download has completed successfully
                Response.SetCookie(new HttpCookie(FileDownloadCookieName, "true") { Path = "/" });
            else
                //ensure that the cookie is removed in case someone did a file download without using jquery.fileDownload
            if (Request.Cookies[FileDownloadCookieName] != null)
                Response.Cookies[FileDownloadCookieName].Expires = DateTime.Now.AddYears(-1);
        }   

        protected void ListarStatus(bool addSelectItem =false)
        {
            var stts = _appsvcStatus.List().ToList().Select(x => new SelectListItem
            {
                Value = x.StatusId.ToString(),
                Text = x.Descricao
            }).ToList();

            if(addSelectItem)
                stts.Insert(0, new SelectListItem
                {
                    Value = (-1).ToString(),
                    Text = "Selecione"
                });


            ViewBag.Status = stts;
        }

        protected void ListarUfs()
        {
            var lst = (new[]
            {
                "AC", "AP", "AM", "RR", "RO", "PA", "TO",
                "MA", "PI", "AL", "CE", "PE", "SE", "BA", "RN","PB",
                "GO", "MT", "MS", "DF",
                "SP", "MG", "RJ", "ES",
                "PR", "SC", "RS"
            }).OrderBy(x => x).Select(x => new SelectListItem { Value = x, Text = x }).ToList();
            lst.Insert(0, new SelectListItem { Value = null, Text = "Selecione" });

            ViewBag.Ufs = lst;
        }

        protected IPagedList<T> CriarPaginador<T>(IEnumerable<T> lst, int p, int pp, string termo) where T: new()
        {
            var model = lst.ToPagedList(p, pp);
            ViewBag.Pagina = p;
            ViewBag.PorPagina = pp;
            ViewBag.Termo = termo;
            ViewBag.Paginas = model.PageCount;
            ViewBag.Total = model.TotalItemCount;
            return model;
        }

        //public BaseController()
        //{

        //}

        public BaseController(IAppServiceBase<Status> appsvcStatus) /*: this()*/
        {
            _appsvcStatus = appsvcStatus;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Usuario = Session["Usuario"] as Usuario;
            //ViewBag.Usuario = Usuario;

            if (Usuario == null)
            {
                filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
            }

            //var rbattrs = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(RibbonTabAttribute),
            //    false);

            //if (rbattrs.Length == 1)
            //    ViewBag.CurrentTab = ((RibbonTabAttribute) rbattrs[0]).Tab;


            base.OnActionExecuting(filterContext);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            CheckAndHandleFileResult(filterContext);
            base.OnResultExecuting(filterContext);
        }

        public ContentResult BuscarCep(string cep)
        {
            try
            {
                cep = cep.ClearNumber();
                var url = ConfigurationManager.AppSettings["ViaCepUrl"];

                var wc = new WebClient();
                wc.Encoding = Encoding.UTF8;

                var req = wc.DownloadString(string.Format(url, cep));

                return Content(req, "application/json");
            }
            catch (Exception e)
            {
                var obj = JsonConvert.SerializeObject(new { error = true, msg = e.Message});
                return Content(obj, "application/json");
            }
        }

        public ContentResult BuscaCnpj(string cnpj)
        {
            try
            {
                cnpj = cnpj.ClearNumber();
                var url = ConfigurationManager.AppSettings["ReceitaWSUrl"];

                var wc = new WebClient();
                wc.Encoding = Encoding.UTF8;

                var req = wc.DownloadString(string.Format(url, cnpj));
                return Content(req, "application/json");
            }
            catch (Exception e)
            {
                var obj = JsonConvert.SerializeObject(new { error = true, msg = e.Message });
                return Content(obj, "application/json");
            }
        }
    }
}