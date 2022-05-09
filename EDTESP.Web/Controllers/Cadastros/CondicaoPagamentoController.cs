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
    public class CondicaoPagamentoController : BaseController
    {
        private readonly ICondicaoPagamentoAppService _appsvcCond;

        private void ListarFormasPgto(bool addSelecione = false)
        {
            var list = _appsvcCond.WhereFormasPgto(x => !x.Removido).Select(x => new SelectListItem
            {
                Value = x.FormaPagamentoId.ToString(),
                Text = x.Descricao
            }).ToList();

            if (addSelecione)
                list.Insert(0, new SelectListItem
                {
                    Value = null,
                    Text = "Selecione",
                    Disabled = true
                });

            ViewBag.FormasPgto = list;
        }

        private string ValidaModelo(string modelo)
        {
            var sp = modelo.Split(',');

            var ns = new List<int>();
            try
            {
                foreach (var i in sp)
                {
                    var n = Convert.ToInt32(i);
                    ns.Add(n);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Modelo inválido");
            }

            return ns.OrderBy(x => x).Select(x => x.ToString()).Aggregate((a, b) => a + "," + b);
        }

        public CondicaoPagamentoController(IAppServiceBase<Status> appsvcStatus, ICondicaoPagamentoAppService appsvcCond) : base(appsvcStatus)
        {
            _appsvcCond = appsvcCond;
        }
        
        [Route("condicoes")]
        [EdtespAuthorize(Roles = "GERCPG")]
        public ActionResult Index(int p = 1, int pp = 20, string termo = null)
        {
            var list = new List<CondicaoPagamentoViewModel>();
            if (!string.IsNullOrEmpty(termo))
            {
                var lst = _appsvcCond.Where(x => !x.Removido && x.Descricao.ToLower().Contains(termo.ToLower())).ToList();
                list = Mapper.Map<List<CondicaoPagamentoViewModel>>(lst);
            }

            var model = CriarPaginador(list, p, pp, termo);
            return View(model);
        }

        [Route("NovaCondicao")]
        [EdtespAuthorize(Roles = "MODCPG")]
        public ActionResult Novo()
        {
            ListarFormasPgto();
            return View();
        }

        [Route("NovaCondicao")]
        [EdtespAuthorize(Roles = "MODCPG")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Novo(CondicaoPagamentoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = Mapper.Map<CondicaoPagamento>(model);
                obj.UsuarioCriadorId = Usuario.UsuarioId;
                _appsvcCond.Insert(obj,model.FormasPgto);
                return RedirectToAction("Condicoes", "Cadastros", new { termo = model.Descricao });
            }
            catch (Exception ex)
            {
                ListarFormasPgto();
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [Route("EditarCondicao/{id}")]
        [EdtespAuthorize(Roles = "MODCPG")]
        public ActionResult Editar(int id)
        {
            var obj = _appsvcCond.Get(id);

            if (obj == null)
                return RedirectToAction("Condicoes", "Cadastros");
            
            ListarFormasPgto();
            return View(Mapper.Map<CondicaoPagamentoViewModel>(obj));
        }

        [Route("EditarCondicao/{id}")]
        [EdtespAuthorize(Roles = "MODCPG")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(CondicaoPagamentoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = _appsvcCond.Get(model.CondicaoPagamentoId);
                obj = Mapper.Map(model, obj);
                obj.UsuarioAtualizadorId = Usuario.UsuarioId;
                obj.DataAtualizacao = DateTime.Now;
                _appsvcCond.Update(obj, model.FormasPgto);

                return RedirectToAction("Condicoes", "Cadastros");
            }
            catch (Exception e)
            {
                ListarFormasPgto();
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }
        }

        [Route("RemoverCondicao")]
        [EdtespAuthorize(Roles = "MODCPG")]
        [HttpPost]
        public JsonResult Remover(int id)
        {
            try
            {
                var obj = _appsvcCond.Get(id);

                if (obj == null)
                    return Json(new { error = true, message = "Condição de pgto. não encontrado" }, JsonRequestBehavior.AllowGet);

                obj.UsuarioAtualizadorId = Usuario.UsuarioId;
                obj.DataAtualizacao = DateTime.Now;
                _appsvcCond.Delete(obj.CondicaoPagamentoId);

                return Json(new { error = false, message = "Condição de pgto. removida" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("ValidarModelo")]
        public JsonResult ValidarModelo(string modelo)
            {
            return Json(_appsvcCond.ValidarModelo(modelo), JsonRequestBehavior.AllowGet);
        }
    }
}