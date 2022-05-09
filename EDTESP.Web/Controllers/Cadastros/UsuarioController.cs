using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Web.Helpers;
using EDTESP.Web.ViewModels;
using EDTESP.Web.ViewModels.Cadastros;
using PagedList;

// ReSharper disable All

namespace EDTESP.Web.Controllers.Cadastros
{
    [RibbonTab(Tab = "cadastro")]
    [RoutePrefix("Cadastros")]
    public class UsuarioController : BaseController
    {
        private IAppServiceBase<GrupoUsuario> _appsvcGrpUsr;
        private IAppServiceBase<PermissaoUsuario> _appsvcPrmsUsr;
        private IAppServiceBase<Permissao> _appsvcPrms;
        private IUsuarioAppService _appsvcUsuario;

        public UsuarioController(IAppServiceBase<Status> appsvcStatus,
            IUsuarioAppService appsvcUsuario,
            IAppServiceBase<GrupoUsuario> appsvcGrpUsr,
            IAppServiceBase<PermissaoUsuario> appsvcPrmsUsr,
            IAppServiceBase<Permissao> appsvcPrms) : base(appsvcStatus)
        {
            _appsvcUsuario = appsvcUsuario;
            _appsvcGrpUsr = appsvcGrpUsr;
            _appsvcPrmsUsr = appsvcPrmsUsr;
            _appsvcPrms = appsvcPrms;
        }

        private void ListarGrupos(bool addSelecione = false)
        {
            var grps = _appsvcGrpUsr.List().ToList().Select(x => new SelectListItem
            {
                Value = x.GrupoUsuarioId.ToString(),
                Text = x.Descricao
            }).ToList(); ;

            if (addSelecione)
                grps.Insert(0, new SelectListItem
                {
                    Value = (-1).ToString(),
                    Text = "Selecione"
                });

            ViewBag.Grupos = grps;
        }

        private void ListarPermissoes()
        {
            var prms = _appsvcPrms.Where(x => !x.Removido).OrderBy(x => x.Nome).ProjectToList<PermissaoViewModel>();
            var grps = prms.Select(x => new Tuple<string, string>(x.Categoria, x.Grupo)).OrderBy(x => x).Distinct().ToList();
            var cats = prms.Select(x => x.Categoria).OrderBy(x => x).Distinct().ToList();

            ViewBag.Categorias = cats;
            ViewBag.Grupos = grps;
            ViewBag.Permissoes = prms;
        }

        private void ListarPermissoesUsuario(int usuarioId)
        {
            var usrprms = _appsvcPrmsUsr.Where(x => x.UsuarioId == usuarioId).ProjectToList<PermissaoUsuarioViewModel>();
            ViewBag.PermissoesUsuario = usrprms;
        }

        [Route("Usuarios")]
        [EdtespAuthorize(Roles = "GERUSU")]
        public ActionResult Index(int p = 1, int pp = 20, string termo = null)
        {
            var users = new List<UsuarioViewModel>();
            if (!string.IsNullOrEmpty(termo))
            {
                users = _appsvcUsuario.Where(x => !x.Removido && (string.IsNullOrEmpty(termo)
                                                                  || x.Nome.ToLower().Contains(termo.ToLower())
                                                                  || x.Login.ToLower().Contains(termo.ToLower())))
                    .ProjectToList<UsuarioViewModel>();
            }

            var model = CriarPaginador(users, p, pp, termo);
            return View(model);
        }

        [Route("NovoUsuario")]
        [EdtespAuthorize(Roles = "MODUSU")]
        public ActionResult Novo()
        {
            ListarStatus(true);
            ListarGrupos(true);
            return View();
        }

        [Route("NovoUsuario")]
        [EdtespAuthorize(Roles = "MODUSU")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Novo(UsuarioViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = Mapper.Map<Usuario>(model);
                obj.UsuarioIdCriador = Usuario.UsuarioId;

                _appsvcUsuario.Insert(obj);
                return RedirectToAction("Usuarios", "Cadastros", new { termo = model.Nome});
            }
            catch (Exception ex)
            {
                ListarGrupos(true);
                ListarStatus(true);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [Route("EditarUsuario/{id}")]
        [EdtespAuthorize(Roles = "MODUSU")]
        public ActionResult Editar(int id)
        {
            var obj = _appsvcUsuario.Get(id);

            if (obj == null)
                return RedirectToAction("Usuarios", "Cadastros");

            ListarStatus(true);
            ListarGrupos(true);
            return View(Mapper.Map<EditarUsuarioViewModel>(obj));
        }

        [Route("EditarUsuario/{id}")]
        [EdtespAuthorize(Roles = "MODUSU")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(EditarUsuarioViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = _appsvcUsuario.Get(model.UsuarioId);
                obj.Nome = model.Nome;
                obj.Sobrenome = model.Sobrenome;
                obj.Email = model.Email;
                obj.GrupoUsuarioId = model.GrupoUsuarioId;
                obj.StatusId = model.StatusId;
                _appsvcUsuario.Update(obj);

                return RedirectToAction("Usuarios", "Cadastros");
            }
            catch (Exception e)
            {
                ListarStatus(true);
                ListarGrupos(true);
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }
        }

        [Route("EditarPermissoes/{id}")]
        [EdtespAuthorize(Roles = "MODUSU")]
        public ActionResult EditarPermissoes(int id)
        {
            var obj = _appsvcUsuario.Get(id);

            if (obj == null)
                return RedirectToAction("Usuarios", "Cadastros");

            ListarPermissoes();
            ListarPermissoesUsuario(obj.UsuarioId);

            return View(Mapper.Map<UsuarioViewModel>(obj));
        }

        [Route("EditarPermissoes/{id}")]
        [EdtespAuthorize(Roles = "MODUSU")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPermissoes(int usuarioId, int[] permissaoId = null)
        {
            var obj = _appsvcUsuario.Where(x => x.UsuarioId == usuarioId).ProjectToFirstOrDefault<UsuarioViewModel>();

            try
            {
                if (obj == null)
                    return RedirectToAction("Usuarios", "Cadastros");

                var uprms = _appsvcPrmsUsr.Where(x => x.UsuarioId == obj.UsuarioId).ToList();
                _appsvcPrmsUsr.DeleteMany(uprms);

                if (permissaoId == null)
                    return RedirectToAction("Usuarios", "Cadastros");

                var prms = permissaoId.Select(pid => new PermissaoUsuario { PermissaoId = pid, UsuarioId = obj.UsuarioId }).ToList();
                _appsvcPrmsUsr.AddOrUpdate(prms);

                return RedirectToAction("Usuarios","Cadastros");
            }
            catch (Exception e)
            {
                ListarPermissoes();
                ListarPermissoesUsuario(obj.UsuarioId);
                ModelState.AddModelError(string.Empty, e.Message);
                return View(obj);
            }
        }

        [Route("RemoverUsuario")]
        [EdtespAuthorize(Roles = "MDOUSU")]
        [HttpPost]
        public JsonResult Remover(int id)
        {
            try
            {
                var obj = _appsvcUsuario.Get(id);

                if (obj == null)
                    return Json(new { error = true, message = "Usuário não encontrado" }, JsonRequestBehavior.AllowGet);

                if (obj.UsuarioIdCriador == 0)
                    return Json(new { error = true, message = "Usuário Administrador não pode ser removido" }, JsonRequestBehavior.AllowGet);

                obj.Removido = true;
                _appsvcUsuario.Update(obj);

                return Json(new { error = false, message = "Usuário removido" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}