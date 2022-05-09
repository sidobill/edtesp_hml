using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Web.Helpers;
using EDTESP.Web.ViewModels;
using EDTESP.Web.ViewModels.Cadastros;
using PagedList;

namespace EDTESP.Web.Controllers.Cadastros
{
    [RibbonTab(Tab = "cadastro")]
    [RoutePrefix("Cadastros")]
    public class GrupoUsuarioController : BaseController
    {
        private IGrupoUsuarioAppService _appsvcGrpUsr;
        private IAppServiceBase<PermissaoGrupoUsuario> _appsvcPrmGrpUsr;
        private IAppServiceBase<Permissao> _appsvcPrms;

        public GrupoUsuarioController(IAppServiceBase<Status> appsvcStt,
            IGrupoUsuarioAppService appsvcGrpUsr, IAppServiceBase<PermissaoGrupoUsuario> appsvcPrmGrpUsr, 
            IAppServiceBase<Permissao> appsvcPrms) : base(appsvcStt)
        {
            _appsvcGrpUsr = appsvcGrpUsr;
            _appsvcPrmGrpUsr = appsvcPrmGrpUsr;
            _appsvcPrms = appsvcPrms;
        }

        private void ListarPermissoes()
        {
            var prms = _appsvcPrms.Where(x => !x.Removido).OrderBy(x => x.Nome).ProjectToList<PermissaoViewModel>();
            var grps = prms.Select(x => new Tuple<string, string>(x.Categoria, x.Grupo) ).OrderBy(x => x).Distinct().ToList();
            var cats = prms.Select(x => x.Categoria).OrderBy(x => x).Distinct().ToList();

            ViewBag.Categorias = cats;
            ViewBag.Grupos = grps;
            ViewBag.Permissoes = prms;
        }

        private void ListarPermissoesGrupo(int grupoId)
        {
            var usrprms = _appsvcPrmGrpUsr.Where(x => x.GrupoUsuarioId == grupoId).ProjectToList<PermissaoGrupoUsuarioVewModel>();
            ViewBag.PermissoesGrupo = usrprms;
        } 

        [Route("Grupos")]
        [EdtespAuthorize(Roles = "GERGRP")]
        public ActionResult Index(int p = 1, int pp = 20, string termo = null)
        {
            var grps = new List<GrupoUsuarioViewModel>();
            if (!string.IsNullOrEmpty(termo))
            {
                grps = _appsvcGrpUsr
                    .Where(x => !x.Removido &&
                                (string.IsNullOrEmpty(termo) || x.Descricao.ToLower().Contains(termo.ToLower())))
                    .ProjectToList<GrupoUsuarioViewModel>();
            }

            var model = CriarPaginador(grps, p, pp, termo);
            return View(model);
        }

        [Route("NovoGrupo")]
        [EdtespAuthorize(Roles = "MODGRP")]
        public ActionResult Novo()
        {
            return View();
        }

        [Route("NovoGrupo")]
        [EdtespAuthorize(Roles = "MODGRP")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Novo(GrupoUsuarioViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = Mapper.Map<GrupoUsuario>(model);
                obj.UsuarioCriadorId = Usuario.UsuarioId;

                _appsvcGrpUsr.Insert(obj);
                return RedirectToAction("Grupos", "Cadastros", new { termo = model.Descricao });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [Route("EditarGrupo/{id}")]
        [EdtespAuthorize(Roles = "MODGRP")]
        public ActionResult Editar(int id)
        {
            var obj = _appsvcGrpUsr.Get(id);

            if (obj == null)
                return RedirectToAction("Grupos", "Cadastros");
            
            return View(Mapper.Map<GrupoUsuarioViewModel>(obj));
        }

        [Route("EditarGrupo/{id}")]
        [EdtespAuthorize(Roles = "MODGRP")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(GrupoUsuarioViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = _appsvcGrpUsr.Get(model.GrupoUsuarioId);
                obj.Descricao = model.Descricao;
                
                _appsvcGrpUsr.Update(obj);

                return RedirectToAction("Grupos", "Cadastros");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }
        }

        [Route("EditarPermissoesGrupo/{id}")]
        [EdtespAuthorize(Roles = "MODGRP")]
        public ActionResult EditarPermissoes(int id)
        {
            var obj = _appsvcGrpUsr.Get(id);

            if (obj == null)
                return RedirectToAction("Grupos", "Cadastros");

            ListarPermissoes();
            ListarPermissoesGrupo(obj.GrupoUsuarioId);

            return View(Mapper.Map<GrupoUsuarioViewModel>(obj));
        }

        [Route("EditarPermissoesGrupo/{id}")]
        [EdtespAuthorize(Roles = "MODGRP")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPermissoes(int grupoUsuarioId, int[] permissaoId = null)
        {
            var obj = _appsvcGrpUsr.Where(x => x.GrupoUsuarioId == grupoUsuarioId).ProjectToFirstOrDefault<GrupoUsuarioViewModel>();

            try
            {
                if (obj == null)
                    return RedirectToAction("Grupos", "Cadastros");

                var uprms = _appsvcPrmGrpUsr.Where(x => x.GrupoUsuarioId == obj.GrupoUsuarioId).ToList();
                _appsvcPrmGrpUsr.DeleteMany(uprms);

                if (permissaoId == null)
                    return RedirectToAction("Grupos", "Cadastros");

                var prms = permissaoId.Select(pid => new PermissaoGrupoUsuario() { PermissaoId = pid, GrupoUsuarioId = obj.GrupoUsuarioId }).ToList();
                _appsvcPrmGrpUsr.AddOrUpdate(prms);

                return RedirectToAction("Grupos", "Cadastros");
            }
            catch (Exception e)
            {
                ListarPermissoes();
                ListarPermissoesGrupo(obj.GrupoUsuarioId);
                ModelState.AddModelError(string.Empty, e.Message);
                return View(obj);
            }
        }

        [Route("RemoverGrupo")]
        [EdtespAuthorize(Roles = "MODGRP")]
        [HttpPost]
        public JsonResult Remover(int id)
        {
            try
            {
                var obj = _appsvcGrpUsr.Get(id);

                if (obj == null)
                    return Json(new { error = true, message = "Grupo não encontrado" }, JsonRequestBehavior.AllowGet);

                if (obj.GrupoUsuarioId == 1)
                    return Json(new { error = true, message = "Grupo Administradores não pode ser removido" }, JsonRequestBehavior.AllowGet);

                obj.Removido = true;
                _appsvcGrpUsr.Update(obj);

                return Json(new { error = false, message = "Grupo removido" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}