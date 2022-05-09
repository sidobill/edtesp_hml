using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Web.Helpers;
using EDTESP.Web.ViewModels.Cadastros;
using PagedList;

namespace EDTESP.Web.Controllers.Cadastros
{
    public class CadastrosController : BaseController
    {
        private IAppServiceBase<Time> _appsvcTime;
        private IAppServiceBase<Cargo> _appsvcCargo;
        private IAppServiceBase<Setor> _appsvcSetor;
        private IAppServiceBase<Feriado> _appsvcFer;

        public CadastrosController(IAppServiceBase<Status> appBase,
            IAppServiceBase<Time> appsvcTime,
            IAppServiceBase<Cargo> appsvcCargo,
            IAppServiceBase<Setor> appsvcSetor,
            IAppServiceBase<Feriado> appsvcFer
        ) : base(appBase)
        {
            _appsvcTime = appsvcTime;
            _appsvcCargo = appsvcCargo;
            _appsvcSetor = appsvcSetor;
            _appsvcFer = appsvcFer;
        }

        #region Cargos
        [EdtespAuthorize(Roles = "GERCRG")]
        public ActionResult Cargos(int p = 1, int pp = 20, string termo = null)
        {
            var lst = new List<CargoViewModel>();
            if (!string.IsNullOrEmpty(termo))
            {
                lst = _appsvcCargo
                    .Where(x => !x.Removido &&
                                (string.IsNullOrEmpty(termo) || x.Descricao.ToLower().Contains(termo.ToLower())))
                    .ProjectToList<CargoViewModel>();
            }

            var model = CriarPaginador(lst, p, pp, termo);
            return View(model);
        }

        [EdtespAuthorize(Roles = "MODCRG")]
        public ActionResult NovoCargo()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EdtespAuthorize(Roles = "MODCRG")]
        public ActionResult NovoCargo(CargoViewModel model)
        {
            try
            {
                if(!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = Mapper.Map<Cargo>(model);
                _appsvcCargo.Insert(obj);

                return RedirectToAction("Cargos", "Cadastros", new { termo = model.Descricao });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }

        }

        [EdtespAuthorize(Roles = "MODCRG")]
        public ActionResult EditarCargo(int id)
        {
            var obj = _appsvcCargo.Get(id);

            if (obj == null)
                return RedirectToAction("Cargos", "Cadastros");

            return View(Mapper.Map<CargoViewModel>(obj));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EdtespAuthorize(Roles = "MODCRG")]
        public ActionResult EditarCargo(CargoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = _appsvcCargo.Get(model.CargoId);
                obj = Mapper.Map(model, obj);
                _appsvcCargo.Update(obj);

                return RedirectToAction("Cargos", "Cadastros");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }

        }

        [EdtespAuthorize(Roles = "MODCRG")]
        [HttpPost]
        public JsonResult RemoverCargo(int id)
        {
            try
            {
                var obj = _appsvcCargo.Get(id);

                if (obj == null)
                    return Json(new { error = true, message = "Cargo não encontrado" }, JsonRequestBehavior.AllowGet);

                obj.Removido = true;
                _appsvcCargo.Update(obj);

                return Json(new { error = false, message = "Cargo removido" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Setores
        [EdtespAuthorize(Roles = "GERSET")]
        public ActionResult Setores(int p = 1, int pp = 20, string termo = null)
        {
            var lst = new List<SetorViewModel>();
            if (!string.IsNullOrEmpty(termo))
            {
                lst = _appsvcSetor
                    .Where(x => !x.Removido &&
                                (string.IsNullOrEmpty(termo) || x.Descricao.ToLower().Contains(termo.ToLower())))
                    .ProjectToList<SetorViewModel>();
            }

            var model = CriarPaginador(lst, p, pp, termo);
            return View(model);
        }

        [EdtespAuthorize(Roles = "MODSET")]
        public ActionResult NovoSetor()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EdtespAuthorize(Roles = "MODSET")]
        public ActionResult NovoSetor(SetorViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = Mapper.Map<Setor>(model);
                _appsvcSetor.Insert(obj);

                return RedirectToAction("Setores", "Cadastros", new { termo = model.Descricao });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }

        }

        [EdtespAuthorize(Roles = "MODSET")]
        public ActionResult EditarSetor(int id)
        {
            var obj = _appsvcSetor.Get(id);

            if (obj == null)
                return RedirectToAction("Setores", "Cadastros");

            return View(Mapper.Map<SetorViewModel>(obj));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EdtespAuthorize(Roles = "MODSET")]
        public ActionResult EditarSetor(SetorViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = _appsvcSetor.Get(model.SetorId);
                obj = Mapper.Map(model, obj);
                _appsvcSetor.Update(obj);

                return RedirectToAction("Setores", "Cadastros");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }

        }

        [EdtespAuthorize(Roles = "MODSET")]
        [HttpPost]
        public JsonResult RemoverSetor(int id)
        {
            try
            {
                var obj = _appsvcSetor.Get(id);

                if (obj == null)
                    return Json(new { error = true, message = "Setor não encontrado" }, JsonRequestBehavior.AllowGet);

                obj.Removido = true;
                _appsvcSetor.Update(obj);

                return Json(new { error = false, message = "Setor removido" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Times
        [EdtespAuthorize(Roles = "GERTIM")]
        public ActionResult Times(int p = 1, int pp = 20, string termo = null)
        {
            var lst = new List<TimeViewModel>();
            if (!string.IsNullOrEmpty(termo))
            {
                lst = _appsvcTime
                    .Where(x => !x.Removido &&
                                (string.IsNullOrEmpty(termo) || x.Descricao.ToLower().Contains(termo.ToLower())))
                    .ProjectToList<TimeViewModel>();
            }

            var model = CriarPaginador(lst, p, pp, termo);
            return View(model);
        }

        [EdtespAuthorize(Roles = "MODTIM")]
        public ActionResult NovoTime()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EdtespAuthorize(Roles = "MODTIM")]
        public ActionResult NovoTime(TimeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = Mapper.Map<Time>(model);
                _appsvcTime.Insert(obj);

                return RedirectToAction("Times", "Cadastros", new { termo = model.Descricao });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }

        }

        [EdtespAuthorize(Roles = "MODTIM")]
        public ActionResult EditarTime(int id)
        {
            var obj = _appsvcTime.Get(id);

            if (obj == null)
                return RedirectToAction("Times", "Cadastros");

            return View(Mapper.Map<TimeViewModel>(obj));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EdtespAuthorize(Roles = "MODTIM")]
        public ActionResult EditarTime(TimeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = _appsvcTime.Get(model.TimeId);
                obj = Mapper.Map(model, obj);
                _appsvcTime.Update(obj);

                return RedirectToAction("Times", "Cadastros");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }

        }

        [EdtespAuthorize(Roles = "MODTIM")]
        [HttpPost]
        public JsonResult RemoverTime(int id)
        {
            try
            {
                var obj = _appsvcTime.Get(id);

                if (obj == null)
                    return Json(new { error = true, message = "Time não encontrado" }, JsonRequestBehavior.AllowGet);

                obj.Removido = true;
                _appsvcTime.Update(obj);

                return Json(new { error = false, message = "Time removido" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Feriados
        [EdtespAuthorize(Roles = "GERFER")]
        public ActionResult Feriados(int p = 1, int pp = 20, string termo = null)
        {
            DateTime.TryParse(termo, out var dt);

            var lst = _appsvcFer
                .Where(x => (string.IsNullOrEmpty(termo) || x.Descricao.ToLower().Contains(termo.ToLower()))
                            || x.Data == dt)
                .ProjectToList<FeriadoViewModel>();

            var model = CriarPaginador(lst, p, pp, termo);
            return View(model);
        }

        [EdtespAuthorize(Roles = "MODFER")]
        public ActionResult NovoFeriado()
        {
            var model = new FeriadoViewModel
            {
                Data = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EdtespAuthorize(Roles = "MODFER")]
        public ActionResult NovoFeriado(FeriadoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = Mapper.Map<Feriado>(model);
                _appsvcFer.Insert(obj);

                return RedirectToAction("Feriados", "Cadastros", new { termo = model.Descricao });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }

        }

        [EdtespAuthorize(Roles = "MODFER")]
        public ActionResult EditarFeriado(int id)
        {
            var obj = _appsvcFer.Get(id);

            if (obj == null)
                return RedirectToAction("Feriados", "Cadastros");

            return View(Mapper.Map<FeriadoViewModel>(obj));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EdtespAuthorize(Roles = "MODFER")]
        public ActionResult EditarFeriado(FeriadoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = _appsvcFer.Get(model.FeriadoId);
                obj = Mapper.Map(model, obj);
                _appsvcFer.Update(obj);

                return RedirectToAction("Feriados", "Cadastros");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }

        }

        [EdtespAuthorize(Roles = "MODFER")]
        [HttpPost]
        public JsonResult RemoverFeriado(int id)
        {
            try
            {
                var obj = _appsvcFer.Get(id);

                if (obj == null)
                    return Json(new { error = true, message = "Time não encontrado" }, JsonRequestBehavior.AllowGet);

                _appsvcFer.Delete(obj);

                return Json(new { error = false, message = "Time removido" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}