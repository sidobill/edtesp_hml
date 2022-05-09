using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EDTESP.Application;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Repositories;
using EDTESP.Web.Helpers;
using EDTESP.Web.ViewModels;

namespace EDTESP.Web.Controllers
{
    public class LoginController : Controller
    {
        private IUsuarioAppService _userAppSvc;

        public LoginController(IUsuarioAppService userAppSvc)
        {
            _userAppSvc = userAppSvc;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                if(!ModelState.IsValid)
                    throw new Exception("Todos os campos são necessários");

                var usr = _userAppSvc.Autenticar(model.Usuario, model.Senha);

                if(usr == null)
                    throw new Exception("usuário e/ou senha inválidos");

                FormsAuthentication.SetAuthCookie(usr.Login,true);
                Session["Usuario"] = usr;

                returnUrl = !string.IsNullOrEmpty(returnUrl) ? returnUrl : FormsAuthentication.DefaultUrl;
                return Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            FormsAuthentication.SignOut();
            return Redirect(FormsAuthentication.LoginUrl);
        }
    }
}