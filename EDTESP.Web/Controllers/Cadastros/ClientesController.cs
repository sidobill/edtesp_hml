using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using EDTESP.Application;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Entities.Edtesp;
using EDTESP.Infrastructure.CC.Util;
using EDTESP.Web.Helpers;
using EDTESP.Web.ViewModels;
using EDTESP.Web.ViewModels.Cadastros;
using Newtonsoft.Json;
using StringUtil = EDTESP.Infrastructure.CC.Util.StringUtil;

namespace EDTESP.Web.Controllers.Cadastros
{
    [RibbonTab(Tab = "cadastro")]
    public class ClientesController : BaseController
    {
        private readonly IAppServiceBase<Cliente> _appsvcCli;
        private readonly IEdtespAppServiceBase<ClassificadosEdtesp> _appsvcClas;
        private readonly IIntegracaoBraslinkApp _integracaoBraslinkApp;

        public ClientesController(AppServiceBase<Status> appsvcStatus, 
            IAppServiceBase<Cliente> appsvcCli,
            IEdtespAppServiceBase<ClassificadosEdtesp> appsvcClas, IIntegracaoBraslinkApp integracaoBraslinkApp) : base(appsvcStatus)
        {
            _appsvcCli = appsvcCli;
            _appsvcClas = appsvcClas;
            _integracaoBraslinkApp = integracaoBraslinkApp;
        }

        [EdtespAuthorize(Roles = "GERCLI")]
        public ActionResult Index(int p = 1, int pp = 20, string termo = null)
        {
            var clis = new List<ClienteViewModel>();
            if (!string.IsNullOrEmpty(termo))
            {
                var ntermo = termo.ClearNumber();
                clis = _appsvcCli.Where(x => !x.Removido && (string.IsNullOrEmpty(termo)
                                                                  || x.RazaoSocial.ToLower().Contains(termo.ToLower())
                                                                  || x.Fantasia.ToLower().Contains(termo.ToLower())
                                                                  || (!string.IsNullOrEmpty(ntermo) &&
                                                                      x.Documento.Contains(ntermo))))
                    .ProjectToList<ClienteViewModel>();
            }

            var model = CriarPaginador(clis, p, pp, termo);
            return View(model);
        }

        [EdtespAuthorize(Roles = "MODCLI")]
        public ActionResult Novo()
        {
            var model = new ClienteViewModel
            {
                StatusId = 1,
                TipoPessoa = "J"
            };

            ListarStatus(true);
            ListarUfs();
            return View(model);
        }

        [EdtespAuthorize(Roles = "MODCLI")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Novo(ClienteViewModel model)
        {
            try
            {
                if(!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");
                
                var obj = Mapper.Map<Cliente>(model);
                obj.UsuarioCriadorId = Usuario.UsuarioId;

                if (model.Logo != null)
                {
                    var finfo = new FileInfo(model.Logo.FileName);
                    var fname = $"{DateTime.Now:ddMMyyyy_HHmmss} {StringUtil.RandomString(32)}{finfo.Extension}";
                    var pathsrv = Path.Combine(Server.MapPath("~//ImagensLogos//"), fname);
                    model.Logo.SaveAs(pathsrv);
                    obj.Logotipo = fname;
                }

                _appsvcCli.Insert(obj);

                return RedirectToAction("Index", "Clientes", new { termo = model.RazaoSocial });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                ListarUfs();
                ListarStatus(true);
                return View(model);
            }
        }

        [EdtespAuthorize(Roles = "MODCLI")]
        public ActionResult Editar(int id)
        {
            var cli = _appsvcCli.Get(id);

            if (cli == null)
                return RedirectToAction("Index", "Clientes");

            ListarStatus(true);
            ListarUfs();
            return View(Mapper.Map<ClienteViewModel>(cli));
        }

        [EdtespAuthorize(Roles = "MODCLI")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(ClienteViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                var obj = _appsvcCli.Get(model.ClienteId);

                obj = Mapper.Map(model, obj);
                obj.UsuarioAtualizadorId = Usuario.UsuarioId;
                obj.UltimaAtualizacao = DateTime.Now;

                if (model.Logo != null)
                {
                    var path = Server.MapPath("~//ImagensLogos//");

                    if (!string.IsNullOrEmpty(obj.Logotipo))
                        System.IO.File.Delete(Path.Combine(path, obj.Logotipo));

                    var finfo = new FileInfo(model.Logo.FileName);
                    var fname = $"{DateTime.Now:ddMMyyyy_HHmmss} {StringUtil.RandomString(32)}{finfo.Extension}";
                    var pathsrv = Path.Combine(path, fname);
                    model.Logo.SaveAs(pathsrv);
                    obj.Logotipo = fname;
                }

                _appsvcCli.Update(obj);

                return RedirectToAction("Index", "Clientes");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                ListarUfs();
                ListarStatus(true);
                return View(model);
            }
        }

        [HttpPost]
        [EdtespAuthorize(Roles = "MODCLI")]
        public JsonResult Remover(int id)
        {
            try
            {
                var obj = _appsvcCli.Get(id);

                if (obj == null)
                    return Json(new { error = true, message = "Cliente não encontrado" }, JsonRequestBehavior.AllowGet);
                
                obj.Removido = true;
                _appsvcCli.Update(obj);

                return Json(new { error = false, message = "Cliente removido" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [EdtespAuthorize(Roles = "GERCLI")]
        public ActionResult Ver(int id)
        {
            var cli = _appsvcCli.Get(id);

            if (cli == null)
                return RedirectToAction("Index", "Clientes");

            ListarStatus(true);
            ListarUfs();
            return View(Mapper.Map<ClienteViewModel>(cli));
        }

        public FileResult ObterLogotipo(int id)
        {
            var cli = _appsvcCli.Get(id);

            if (cli == null || string.IsNullOrEmpty(cli.Logotipo))
                return null;
            
            var path = Server.MapPath("~/app_data/logotipos");
            var str = System.IO.File.OpenRead(Path.Combine(path, cli.Logotipo));
            var finfo = new FileInfo(cli.Logotipo);

            var mt = MimeMapping.GetMimeMapping(finfo.FullName);
            return File(str, mt, $"imagem{finfo.Extension}");
        }

        [HttpPost]
        public JsonResult UploadCliente(int clienteId, int classId)
        {
            try
            {
                var cli = _appsvcCli.Get(clienteId);

                if (cli == null)
                    throw new Exception("Cliente não encontrado");

                var scli = new Helpers.JsonModels.Clientes();

                scli.Lbsid = "ADADDAD45645";
                scli.CategoriaId = classId;
                scli.Nome = cli.Fantasia;
                scli.Logradouro = string.Empty;
                scli.Endereco = cli.Endereco;
                scli.Numero = cli.Numero;
                scli.Complemento = cli.Complemento;
                scli.Bairro = cli.Bairro;
                scli.Cidade = cli.Cidade;
                scli.Uf = cli.Uf;
                scli.Cep = cli.Cep;

                var ddd = cli.Telefone.Substring(0, 2);
                scli.Ddd = ddd;
                scli.Telefone = cli.Telefone.Substring(2);
                scli.Telefone2 = !string.IsNullOrEmpty(cli.Celular) ? cli.Celular.Substring(2) : null;
                scli.Telefone2 = !string.IsNullOrEmpty(cli.TelefoneOutro) ? cli.TelefoneOutro.Substring(2) : null;
                scli.Fax = !string.IsNullOrEmpty(cli.Fax) ? cli.Fax.Substring(2) : null;

                scli.Site = cli.Website;
                scli.Email = cli.Email;
                scli.Letra = cli.Fantasia.Substring(0, 1).ToUpper();
                scli.Anuncio = cli.Logotipo;

                IntegracaoHelper.InserirCliente(scli);

                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DocumentoValido(string documento, string tipoPessoa, int clienteId = 0)
        {
            var doc = documento.ClearNumber();

            if(doc.Length != 11 && doc.Length != 14)
                return Json(true, JsonRequestBehavior.AllowGet);

            if (tipoPessoa == "F" && !doc.IsCpf())
                return Json(false, JsonRequestBehavior.AllowGet);

            if (tipoPessoa == "J" && !doc.IsCnpj())
                return Json(false, JsonRequestBehavior.AllowGet);

            var obj = _appsvcCli.Where(x => !x.Removido && x.Documento == doc && x.ClienteId != clienteId)
                .FirstOrDefault();

            return Json(obj == null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EnvioSite()
        {
            var clas = IntegracaoHelper.ObterClassificados().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Categoria}).ToList();
            clas.Insert(0, new SelectListItem { Value = "", Text = "Selecione"});
            ViewBag.Classifs = clas;

            return View("_EnvioSite");
        }

        public ActionResult IntegrarClienteBraslink(int p = 1, int pp = 20, string nome = null)
        {
            ViewBag.NomeCli = nome;

            var clis = new List<ClienteViewModel>();

            if (!string.IsNullOrEmpty(nome))
            {
                var ntermo = nome.ClearNumber();
                clis = _appsvcCli.Where(x => !x.Removido && (string.IsNullOrEmpty(nome)
                                                                  || x.RazaoSocial.ToLower().Contains(nome.ToLower())
                                                                  || x.Fantasia.ToLower().Contains(nome.ToLower())))
                    .ProjectToList<ClienteViewModel>();
            }

            var clientesIntegrados = _integracaoBraslinkApp.RetornarClientesIntegrados();

            if(clientesIntegrados != null && clientesIntegrados.Any() && clis != null && clis.Any())
            {
                foreach(var item in clis)
                {
                    if (clientesIntegrados.Where(r => r == item.ClienteId).FirstOrDefault() > 0)
                        item.IntegradoBraslink = true;
                }
            }

            var model = CriarPaginador(clis, p, pp, nome);

            return View("IntegrarClienteBraslink", model);
        }

        [HttpPost]
        public JsonResult InserirClienteBraslink(string id)
        {
            var resultado = new RetornoIntegracaoClienteBraslink();

            try
            {
                var caminhoBase = Server.MapPath("~//ImagensLogos//");

                resultado.Sucesso = _integracaoBraslinkApp.IntegrarClienteParaBraslink(Convert.ToInt32(id), caminhoBase);
            }
            catch (Exception ex)
            {
                resultado.Mensagem = ex.Message;
            }

            return Json(resultado, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ExcluirClienteBraslink(string id)
        {
            var resultado = new RetornoIntegracaoClienteBraslink();

            try
            {
                resultado.Sucesso = _integracaoBraslinkApp.ExcluirClienteBraslink(Convert.ToInt32(id));
            }
            catch (Exception ex)
            {
                resultado.Mensagem = ex.Message;
            }

            return Json(resultado, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult AlterarClienteBraslink(string json)
        {
            var resultado = new RetornoIntegracaoClienteBraslink();

            try
            {
                var objetoCliente = JsonConvert.DeserializeObject<ClienteBraslink>(json);

                var caminhoBase = Server.MapPath("~/ImagensLogos");

                resultado.Sucesso = _integracaoBraslinkApp.AlterarClienteParaBraslink(objetoCliente, caminhoBase);
            }
            catch (Exception ex)
            {
                resultado.Mensagem = ex.Message;
            }

            return Json(resultado, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult RetornarDadosClientePeloId(int clienteId)
        {
            var resultado = new RetornoIntegracaoClienteBraslink();

            try
            {
                resultado.ResultadoObjeto = _integracaoBraslinkApp.RetornarClienteBraslinkPeloId(clienteId);

                resultado.Sucesso = true;
            }
            catch (Exception ex)
            {
                resultado.Mensagem = ex.Message;
            }

            return Json(resultado, JsonRequestBehavior.DenyGet);
        }
    }
}