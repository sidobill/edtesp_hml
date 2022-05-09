using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Text;
using AutoMapper;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Infrastructure.CC.Util;
using EDTESP.Web.Helpers;
using EDTESP.Web.ViewModels.Cadastros;
using PagedList;

namespace EDTESP.Web.Controllers.Cadastros
{
    [RibbonTab(Tab = "cadastro")]
    [RoutePrefix("Cadastros")]
    public class VendedorController : BaseController
    {
        private readonly IAppServiceBase<Vendedor> _appsvcVnd;
        private readonly IAppServiceBase<Time> _appsvcTime;
        private readonly IAppServiceBase<Cargo> _appsvcCargo;
        private readonly IAppServiceBase<Setor> _appsvcSetor;
        private readonly IAppServiceBase<VendedorAlcada> _appsvcAlc;
        private readonly IAppServiceBase<StatusVendedor> _appsvcStv;


        public VendedorController(IAppServiceBase<Status> appsvcStatus, 
            IAppServiceBase<Vendedor> appsvcVnd,
            IAppServiceBase<Time> appsvcTime,
            IAppServiceBase<Cargo> appsvcCargo,
            IAppServiceBase<Setor> appsvcSetor,
            IAppServiceBase<VendedorAlcada> appsvcAlc,
            IAppServiceBase<StatusVendedor> appsvcStv
        ) : base(appsvcStatus)
        {
            _appsvcVnd = appsvcVnd;
            _appsvcTime = appsvcTime;
            _appsvcCargo = appsvcCargo;
            _appsvcSetor = appsvcSetor;
            _appsvcAlc = appsvcAlc;
            _appsvcStv = appsvcStv;
        }

        private void ListarStatusVendedores()
        {
            var lst = _appsvcStv.List().ToList().Select(x => new SelectListItem
            {
                Value = x.StatusVendedorId.ToString(),
                Text = x.Descricao
            }).ToList();
            
            lst.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Selecione"
            });

            ViewBag.Status = lst;
        }

        private void ListarTimes(bool addSel = true)
        {
            var lst = _appsvcTime.Where(x => !x.Removido).ToList().Select(x => new SelectListItem
            {
                Value = x.TimeId.ToString(),
                Text = x.Descricao
            }).ToList();

            if(addSel)
                lst.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = "Selecione"
                });

            ViewBag.Times = lst;
        }

        private void ListarCargos(bool addSel = true)
        {
            var lst = _appsvcCargo.Where(x => !x.Removido).ToList().Select(x => new SelectListItem
            {
                Value = x.CargoId.ToString(),
                Text = x.Descricao
            }).ToList();

            if (addSel)
                lst.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = "Selecione"
                });

            ViewBag.Cargos = lst;
        }

        private void ListarSetores(bool addSel = true)
        {
            var lst = _appsvcSetor.Where(x => !x.Removido).ToList().Select(x => new SelectListItem
            {
                Value = x.SetorId.ToString(),
                Text = x.Descricao
            }).ToList();

            if (addSel)
                lst.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = "Selecione"
                });

            ViewBag.Setores = lst;
        }

        private void ListarAlcadas(bool addSel = true)
        {
            var lst = _appsvcAlc.List().Select(x => new SelectListItem
            {
                Value = x.VendedorAlcadaId.ToString(),
                Text = x.Descricao
            }).ToList();

            if (addSel)
                lst.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = "Selecione"
                });

            ViewBag.Alcadas = lst;
        }

        [Route("Vendedores")]
        [EdtespAuthorize(Roles = "GERVND")]
        public ActionResult Index(int p = 1, int pp = 20, string termo = null)
        {
            var vends = new List<VendedorViewModel>();
            if (!string.IsNullOrEmpty(termo))
            {
                var ntermo = termo.ClearNumber();
                vends = _appsvcVnd.Where(x => !x.Removido && (x.Nome.ToLower().Contains(termo.ToLower())
                                                      || x.NomeReduzido.ToLower().Contains(termo.ToLower())
                                                      || (!string.IsNullOrEmpty(ntermo) && x.Cpf.Contains(ntermo))))
                    .ProjectToList<VendedorViewModel>();
            }

            var model = CriarPaginador(vends, p, pp, termo);
            
            return View(model);
        }

        [Route("NovoVendedor")]
        [EdtespAuthorize(Roles = "MODVND")]
        public ActionResult Novo()
        {
            ListarStatusVendedores();
            ListarTimes();
            ListarCargos();
            ListarSetores();
            ListarAlcadas();
            ListarUfs();

            var model = new VendedorViewModel{ StatusVendedorId = 1 };
            return View(model);
        }

        [Route("NovoVendedor")]
        [EdtespAuthorize(Roles = "MODVND")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Novo(VendedorViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = Mapper.Map<Vendedor>(model);
                obj.UsuarioCriadorId = Usuario.UsuarioId;

                _appsvcVnd.Insert(obj);
                return RedirectToAction("Vendedores", "Cadastros", new { termo = model.Nome });
            }
            catch (Exception ex)
            {
                ListarStatusVendedores();
                ListarTimes();
                ListarCargos();
                ListarSetores();
                ListarAlcadas();
                ListarUfs();
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [Route("EditarVendedor/{id}")]
        [EdtespAuthorize(Roles = "MODVND")]
        public ActionResult Editar(int id)
        {
            var obj = _appsvcVnd.Get(id);

            if (obj == null)
                return RedirectToAction("Usuarios", "Cadastros");

            ListarStatusVendedores();
            ListarTimes();
            ListarCargos();
            ListarSetores();
            ListarAlcadas();
            ListarUfs();
            return View(Mapper.Map<VendedorViewModel>(obj));
        }

        [Route("EditarVendedor/{id}")]
        [EdtespAuthorize(Roles = "MODVND")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(VendedorViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = _appsvcVnd.Get(model.VendedorId);
                obj = Mapper.Map(model, obj);
                obj.DataAtualizacao = DateTime.Now;
                obj.UsuarioAtualizadorId = Usuario.UsuarioId;

                _appsvcVnd.Update(obj);

                return RedirectToAction("Vendedores", "Cadastros");
            }
            catch (Exception e)
            {
                ListarStatusVendedores();
                ListarTimes();
                ListarCargos();
                ListarSetores();
                ListarUfs();
                ListarAlcadas();
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }
        }

        [Route("RemoverVendedor")]
        [EdtespAuthorize(Roles = "MODVND")]
        [HttpPost]
        public JsonResult Remover(int id)
        {
            try
            {
                var obj = _appsvcVnd.Get(id);

                if (obj == null)
                    return Json(new { error = true, message = "Vendedor não encontrado" }, JsonRequestBehavior.AllowGet);

                obj.Removido = true;
                _appsvcVnd.Update(obj);

                return Json(new { error = false, message = "Vendedor removido" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("DocumentoExiste")]
        [EdtespAuthorize(Roles = "MODVND")]
        public JsonResult DocumentoExiste(string cpf, int vendedorId = 0)
        {
            var doc = cpf.ClearNumber();

            if (doc.Length != 11)
                return Json(true, JsonRequestBehavior.AllowGet);

            if (!doc.IsCpf())
                return Json(false, JsonRequestBehavior.AllowGet);

            var obj = _appsvcVnd.Where(x => !x.Removido && x.Cpf == doc && x.VendedorId != vendedorId).FirstOrDefault();

            return Json(obj == null, JsonRequestBehavior.AllowGet);
        }
    }
}