using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Web.Helpers;
using EDTESP.Web.ViewModels.Cadastros;
using PagedList;

namespace EDTESP.Web.Controllers.Cadastros
{
    [RibbonTab(Tab = "cadastro")]
    [RoutePrefix("cadastros")]
    public class FormaPagamentoController : BaseController
    {
        private readonly IAppServiceBase<FormaPagamento> _appsvcForma;

        public FormaPagamentoController(IAppServiceBase<Status> appsvcStatus, IAppServiceBase<FormaPagamento> appsvcForma) : base(appsvcStatus)
        {
            _appsvcForma = appsvcForma;
        }
        
        [Route("formas")]
        [EdtespAuthorize(Roles = "GERFPG")]
        public ActionResult Index(int p = 1, int pp = 20, string termo = null)
        {
            var list = new List<FormaPagamentoViewModel>();

            if (!string.IsNullOrEmpty(termo))
            {
                list = _appsvcForma.Where(x => !x.Removido && x.Descricao.ToLower().Contains(termo.ToLower())).ProjectToList<FormaPagamentoViewModel>();
            }

            var model = CriarPaginador(list, p, pp, termo);
            return View(model);
        }

        [Route("NovaForma")]
        [EdtespAuthorize(Roles = "MODFPG")]
        public ActionResult Novo()
        {
            return View();
        }

        [Route("NovaForma")]
        [EdtespAuthorize(Roles = "MODFPG")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Novo(FormaPagamentoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = Mapper.Map<FormaPagamento>(model);
                obj.UsuarioCriadorId = Usuario.UsuarioId;

                _appsvcForma.Insert(obj);
                return RedirectToAction("Formas", "Cadastros", new { termo = model.Descricao });
            }
            catch (Exception ex)
            {
                ListarStatus(true);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [Route("EditarForma/{id}")]
        [EdtespAuthorize(Roles = "MODFPG")]
        public ActionResult Editar(int id)
        {
            var obj = _appsvcForma.Get(id);

            if (obj == null)
                return RedirectToAction("Formas", "Cadastros");

            ListarStatus(true);
            return View(Mapper.Map<FormaPagamentoViewModel>(obj));
        }

        [Route("EditarForma/{id}")]
        [EdtespAuthorize(Roles = "MODFPG")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(FormaPagamentoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = _appsvcForma.Get(model.FormaPagamentoId);
                obj = Mapper.Map(model, obj);
                obj.UsuarioAtualizadorId = Usuario.UsuarioId;
                obj.UltimaAtualizacao = DateTime.Now;
                _appsvcForma.Update(obj);

                return RedirectToAction("Formas", "Cadastros");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }
        }

        [Route("RemoverForma")]
        [EdtespAuthorize(Roles = "MODFPG")]
        [HttpPost]
        public JsonResult Remover(int id)
        {
            try
            {
                var obj = _appsvcForma.Get(id);

                if (obj == null)
                    return Json(new { error = true, message = "Forma de pgto. não encontrado" }, JsonRequestBehavior.AllowGet);

                obj.UsuarioAtualizadorId = Usuario.UsuarioId;
                obj.UltimaAtualizacao = DateTime.Now;
                obj.Removido = true;
                _appsvcForma.Update(obj);

                return Json(new { error = false, message = "Forma de pgto. removida" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}