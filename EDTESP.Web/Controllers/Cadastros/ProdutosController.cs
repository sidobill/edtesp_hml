using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using EDTESP.Application;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Infrastructure.CC.Util;
using EDTESP.Web.Helpers;
using EDTESP.Web.ViewModels.Cadastros;

namespace EDTESP.Web.Controllers.Cadastros
{
    public class ProdutosController : BaseController
    {
        private readonly IAppServiceBase<Produto> _appsvcPrd;

        public ProdutosController(AppServiceBase<Status> appsvcStatus, IAppServiceBase<Produto> appsvcPrd) : base(appsvcStatus)
        {
            _appsvcPrd = appsvcPrd;
        }

        [EdtespAuthorize(Roles = "GERPRD")]
        public ActionResult Index(int p = 1, int pp = 20, string termo = null)
        {
            var prds = new List<ProdutoViewModel>();
            if (!string.IsNullOrEmpty(termo))
            {
                prds = _appsvcPrd.Where(x => !x.Removido && (string.IsNullOrEmpty(termo)
                                                                  || x.Descricao.ToLower().Contains(termo.ToLower())
                                                                  || x.Categoria.ToLower().Contains(termo.ToLower())
                                                                  || x.Especie.ToLower().Contains(termo.ToLower())))
                    .ProjectToList<ProdutoViewModel>();
            }

            var model = CriarPaginador(prds, p, pp, termo);
            return View(model);
        }

        [EdtespAuthorize(Roles = "MODPRD")]
        public ActionResult Novo()
        {
            return View();
        }

        [EdtespAuthorize(Roles = "MODPRD")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Novo(ProdutoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = Mapper.Map<Produto>(model);
                obj.UsuarioCriadorId = Usuario.UsuarioId;
                _appsvcPrd.Insert(obj);

                return RedirectToAction("Index", "Produtos", new { termo = model.Descricao });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }
        }

        [EdtespAuthorize(Roles = "MODPRD")]
        public ActionResult Editar(int id)
        {
            var prd = _appsvcPrd.Get(id);

            if (prd == null)
                return RedirectToAction("Index", "Produtos");

            return View(Mapper.Map<ProdutoViewModel>(prd));
        }

        [EdtespAuthorize(Roles = "MODPRD")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(ProdutoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = _appsvcPrd.Get(model.ProdutoId);

                obj = Mapper.Map(model, obj);
                obj.UsuarioAtualizadorid = Usuario.UsuarioId;
                obj.UltimaAtualizacao = DateTime.Now;
                _appsvcPrd.Update(obj);

                return RedirectToAction("Index", "Produtos");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }
        }

        [HttpPost]
        [EdtespAuthorize(Roles = "MODPRD")]
        public JsonResult Remover(int id)
        {
            try
            {
                var obj = _appsvcPrd.Get(id);

                if (obj == null)
                    return Json(new { error = true, message = "Produto não encontrado" }, JsonRequestBehavior.AllowGet);

                obj.Removido = true;
                _appsvcPrd.Update(obj);

                return Json(new { error = false, message = "Produto removido" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
    }
}