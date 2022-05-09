using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using EDTESP.Application;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Infrastructure.CC.Util;
using EDTESP.Web.Filters;
using EDTESP.Web.Helpers;
using EDTESP.Web.ViewModels.Financeiro;
using EDTESP.Zip;

namespace EDTESP.Web.Controllers.Financeiro
{
    [RibbonTab(Tab = "financeiro")]
    public class FinanceiroController : BaseController
    {
        private readonly ITituloAppService _appsvcTit;
        private readonly IAppServiceBase<Boleto> _appsvcBol;
        private readonly IAppServiceBase<ParametroBanco> _appsvcPrmBnc;
        private readonly IAppServiceBase<Remessa> _appsvcRem;
        private readonly IAppServiceBase<PermissaoUsuario> _appsvcPrmsUsr;

        public FinanceiroController(AppServiceBase<Status> appsvcbase,
            ITituloAppService appsvcTit,
            IAppServiceBase<Boleto> appsvcBol,
            IAppServiceBase<ParametroBanco> appsvcPrmBnc,
            IAppServiceBase<Remessa> appsvcRem,
            IAppServiceBase<PermissaoUsuario> appsvcPrmsUsr) : base(appsvcbase)
        {
            _appsvcTit = appsvcTit;
            _appsvcBol = appsvcBol;
            _appsvcPrmBnc = appsvcPrmBnc;
            _appsvcRem = appsvcRem;
            _appsvcPrmsUsr = appsvcPrmsUsr;
        }

        private void ListarParametros()
        {
            var lst = _appsvcPrmBnc.List().ToList().Select(x => new SelectListItem
            {
                Value = x.ParametroBancoId.ToString(),
                Text = x.Descricao,
            }).ToList();
            lst.Insert(0,new SelectListItem
            {
                Value = null,
                Text = "Selecione"
            });

            ViewBag.Parametros = lst;
        }

        [EdtespAuthorize(Roles = "GERTIT,GERTITUS")]
        public ActionResult Index(int p = 1, int pp = 20, string termo = null, string emiIni = null, string emiFim = null)
        {
            var tits = new List<TituloViewModel>();
            if (!string.IsNullOrEmpty(termo) || !string.IsNullOrEmpty(emiIni))
            {
                var ntermo = termo.ClearNumber();
                int.TryParse(ntermo, out var itermo);

                var demiini = string.IsNullOrEmpty(emiIni) ? null : (DateTime?)Convert.ToDateTime(emiIni + " 00:00:00");
                var demifim = string.IsNullOrEmpty(emiFim) ? null : (DateTime?)Convert.ToDateTime(emiFim + " 23:59:59");

                ViewBag.Ini = demiini;
                ViewBag.Fim = demifim;

                var usuario = RetornarUsuarioParaFiltrar();

                if (usuario <= 0)
                {
                    tits = _appsvcTit.Where(x =>
                        !x.Removido && (string.IsNullOrEmpty(termo)
                                                                 || x.Cliente.RazaoSocial.ToLower().Contains(termo.ToLower())
                                                                 || (!string.IsNullOrEmpty(x.Cliente.Fantasia) && x.Cliente.Fantasia.ToLower().Contains(termo.ToLower()))
                                                                 || x.Vendedor.Nome.ToLower().Contains(termo.ToLower())
                                                                 || (!string.IsNullOrEmpty(x.Vendedor.NomeReduzido) && x.Vendedor.NomeReduzido.ToLower().Contains(termo.ToLower()))
                                                                 || x.FormaPagamento.Descricao.ToLower().Contains(termo.ToLower())
                                                                 || x.CondicaoPagamento.Descricao.ToLower().Contains(termo.ToLower())
                                                                 || x.Contrato.Numero.Equals(itermo)
                                                                 || x.Boletos.Any(y => y.NossoNumeroBanco.Contains(termo))
                                                                 )
                                        && (!demiini.HasValue || (x.DataCriacao >= demiini && x.DataCriacao <= demifim))
                        ).OrderBy(x => x.TituloId).ProjectToList<TituloViewModel>();
                }
                else
                {
                    tits = _appsvcTit.Where(x =>
                        !x.Removido && x.Contrato != null && x.Contrato.UsuarioCriadorId == usuario && (string.IsNullOrEmpty(termo)
                                                                 || x.Cliente.RazaoSocial.ToLower().Contains(termo.ToLower())
                                                                 || (!string.IsNullOrEmpty(x.Cliente.Fantasia) && x.Cliente.Fantasia.ToLower().Contains(termo.ToLower()))
                                                                 || x.Vendedor.Nome.ToLower().Contains(termo.ToLower())
                                                                 || (!string.IsNullOrEmpty(x.Vendedor.NomeReduzido) && x.Vendedor.NomeReduzido.ToLower().Contains(termo.ToLower()))
                                                                 || x.FormaPagamento.Descricao.ToLower().Contains(termo.ToLower())
                                                                 || x.CondicaoPagamento.Descricao.ToLower().Contains(termo.ToLower())
                                                                 || x.Contrato.Numero.Equals(itermo)
                                                                 || x.Boletos.Any(y => y.NossoNumeroBanco.Contains(termo))
                                                                 )
                                        && (!demiini.HasValue || (x.DataCriacao >= demiini && x.DataCriacao <= demifim))
                        ).OrderBy(x => x.TituloId).ProjectToList<TituloViewModel>();
                }
            }

            if (tits != null && tits.Any())
            {
                var marcarComoPago = this.RetornarPermissaoMarcarComoPago();
                var marcarComoNaoPago = this.RetornarPermissaoMarcarComoNaoPago();

                foreach (var item in tits)
                {
                    item.PermissaoParaMarcarComoPago = marcarComoPago;
                    item.PermissaoParaMarcarComoNaoPago = marcarComoNaoPago;
                }
            }

            var model = CriarPaginador(tits, p, pp, termo);
            return View(model);
        }

        [EdtespAuthorize(Roles = "GERTIT,GERTITUS")]
        public ActionResult EmAtraso(int p = 1, int pp = 20, string termo = null, string emiIni = null, string emiFim = null)
        {
            
            var dlim = DateTime.Now.AddDays(-1);
            
            var ntermo = termo.ClearNumber();
            int.TryParse(ntermo, out var itermo);

            var demiini = string.IsNullOrEmpty(emiIni) ? null : (DateTime?)Convert.ToDateTime(emiIni + " 00:00:00");
            var demifim = string.IsNullOrEmpty(emiFim) ? null : (DateTime?)Convert.ToDateTime(emiFim + " 23:59:59");

            ViewBag.Ini = demiini;
            ViewBag.Fim = demifim;

            var usuario = RetornarUsuarioParaFiltrar();

            var tits = new List<TituloViewModel>();

            if (usuario <= 0)
            {
                tits = _appsvcTit.Where(x => !x.Removido && x.DataVenctoReal < dlim && x.Saldo > 0
                                             && (string.IsNullOrEmpty(termo)
                                                             || x.Cliente.RazaoSocial.ToLower().Contains(termo.ToLower())
                                                             || (!string.IsNullOrEmpty(x.Cliente.Fantasia) && x.Cliente.Fantasia.ToLower().Contains(termo.ToLower()))
                                                             || x.Vendedor.Nome.ToLower().Contains(termo.ToLower())
                                                             || (!string.IsNullOrEmpty(x.Vendedor.NomeReduzido) && x.Vendedor.NomeReduzido.ToLower().Contains(termo.ToLower()))
                                                             || x.FormaPagamento.Descricao.ToLower().Contains(termo.ToLower())
                                                             || x.CondicaoPagamento.Descricao.ToLower().Contains(termo.ToLower())
                                                             || x.Contrato.Numero.Equals(itermo)
                                                             || x.Boletos.Any(y => y.NossoNumeroBanco.Contains(termo))
                                                             || (!demiini.HasValue
                                                                && (x.DataCriacao >= demiini && x.DataCriacao <= demifim))
                                             )).OrderBy(x => x.DataVenctoReal).ProjectToList<TituloViewModel>();


                
            }
            else
            {
                tits = _appsvcTit.Where(x => !x.Removido && x.DataVenctoReal < dlim && x.Saldo > 0  && x.Contrato != null && x.Contrato.UsuarioCriadorId == usuario
                                             && (string.IsNullOrEmpty(termo)
                                                             || x.Cliente.RazaoSocial.ToLower().Contains(termo.ToLower())
                                                             || (!string.IsNullOrEmpty(x.Cliente.Fantasia) && x.Cliente.Fantasia.ToLower().Contains(termo.ToLower()))
                                                             || x.Vendedor.Nome.ToLower().Contains(termo.ToLower())
                                                             || (!string.IsNullOrEmpty(x.Vendedor.NomeReduzido) && x.Vendedor.NomeReduzido.ToLower().Contains(termo.ToLower()))
                                                             || x.FormaPagamento.Descricao.ToLower().Contains(termo.ToLower())
                                                             || x.CondicaoPagamento.Descricao.ToLower().Contains(termo.ToLower())
                                                             || x.Contrato.Numero.Equals(itermo)
                                                             || x.Boletos.Any(y => y.NossoNumeroBanco.Contains(termo))
                                                             || (!demiini.HasValue
                                                                && (x.DataCriacao >= demiini && x.DataCriacao <= demifim))
                                             )).OrderBy(x => x.DataVenctoReal).ProjectToList<TituloViewModel>();
            }

            if(tits != null && tits.Any())
            {
                var marcarComoPago = this.RetornarPermissaoMarcarComoPago();
                var marcarComoNaoPago = this.RetornarPermissaoMarcarComoNaoPago();

                foreach(var item in tits)
                {
                    item.PermissaoParaMarcarComoPago = marcarComoPago;
                    item.PermissaoParaMarcarComoNaoPago = marcarComoNaoPago;
                }
            }

            var model = CriarPaginador(tits, p, pp, termo);

            return View(model);
        }

        [EdtespAuthorize(Roles = "GERTIT")]
        public ActionResult BoletosPendentes(string termo = null)
        {
            var ntermo = termo.ClearNumber();
            var itermo = Convert.ToInt32(!string.IsNullOrEmpty(ntermo)? ntermo: "0");
            var bols = _appsvcBol.Where(x => x.Titulo.FormaPagamento.GeraBoleto && !x.EnviadoAoBanco && !x.Cancelado && !x.Titulo.Suspenso
                                                                && (string.IsNullOrEmpty(termo)
                                                                    || x.Cliente.RazaoSocial.ToLower().Contains(termo.ToLower())
                                                                    || (!string.IsNullOrEmpty(x.Cliente.Fantasia) && x.Cliente.Fantasia.ToLower().Contains(termo.ToLower()))
                                                                    || x.Titulo.ContratoId.Equals(itermo)
                                                                    || (!string.IsNullOrEmpty(x.NossoNumeroBanco) && x.NossoNumeroBanco.Contains(termo))
                                                                    )).OrderBy(x => x.TituloId).ProjectToList<BoletoViewModel>();
            
            ViewBag.Termo = termo;
            ListarParametros();
            return View(bols);
        }

        [ValidateAntiForgeryToken]
        [EdtespAuthorize(Roles = "ADDBOL")]
        [HttpPost]
        public ActionResult GerarCnab(int modalidadeId, int[] boletoId, string termo = null)
        {
            try
            {
                var path = Server.MapPath(EdtespConfig.AppDataFolder);
                _appsvcTit.GerarBoletos(boletoId.ToList(), modalidadeId, Usuario.UsuarioId, path);

                return RedirectToAction("Remessas", "Financeiro");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                var ntermo = termo.ClearNumber();
                var itermo = Convert.ToInt32(!string.IsNullOrEmpty(ntermo) ? ntermo : "0");
                var bols = _appsvcBol.Where(x => !x.EnviadoAoBanco && !x.Cancelado && !x.Titulo.Suspenso
                                                                   && (string.IsNullOrEmpty(termo)
                                                                       || x.Cliente.RazaoSocial.ToLower().Contains(termo.ToLower())
                                                                       || (!string.IsNullOrEmpty(x.Cliente.Fantasia) && x.Cliente.Fantasia.ToLower().Contains(termo.ToLower()))
                                                                       || x.Titulo.ContratoId.Equals(itermo)
                                                                       || (!string.IsNullOrEmpty(x.NossoNumeroBanco) && x.NossoNumeroBanco.Contains(termo))
                                                                   )).OrderBy(x => x.TituloId).ProjectToList<BoletoViewModel>();

                ViewBag.Termo = termo;
                ListarParametros();
                return View("BoletosPendentes", bols);
            }
        }
        
        public ActionResult Ver(int id)
        {
            var tit = _appsvcTit.Get(id);

            if (tit == null)
            {
                Response.StatusCode = (int) HttpStatusCode.NotFound;
                //Response.Status = "Titulo não encotrado";
                return null;
            }

            var model = Mapper.Map<TituloViewModel>(tit);

            if (tit.FormaPagamento != null && tit.FormaPagamento.FormaPagamentoId > 0)
                model.FormaPagamento = tit.FormaPagamento.Descricao;

            return View(model);
        }

        [EdtespAuthorize(Roles = "ADDBOL")]
        public ActionResult Remessas(int p = 1, int pp = 20, string termo = null)
        {
            var lst = _appsvcRem.Where(x => 
                string.IsNullOrEmpty(termo)
                || x.NomeTxt.ToLower().Contains(termo) 
                || x.ParametroBanco.Descricao.ToLower().Contains(termo)).OrderByDescending(x => x.DataCriacao)
                .ProjectToList<RemessaViewModel>();

            var model = CriarPaginador(lst, p, pp, termo);
            return View(model);
        }

        [HttpGet]
        public FileResult BaixarRemessa(int remessaId)
        {
            try
            {
                var rem = _appsvcRem.Get(remessaId);

                if (rem == null)
                    throw new Exception("Remessa não encontrada");
                
                var folderpath = Server.MapPath(EdtespConfig.AppDataFolder);
                var path = Path.Combine(folderpath, rem.NomeTxt);

                if (!System.IO.File.Exists(path))
                    throw new Exception("Remessa não exista");

                if (!rem.Baixada)
                {
                    rem.Baixada = true;
                    _appsvcRem.Update(rem);
                }

                return File(path, "text/plain", rem.NomeTxt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet, FileDownload]
        public FileResult BaixarBoleto(int boletoId)
        {
            try
            {
                var bol = _appsvcBol.Get(boletoId);

                if(bol == null)
                    throw new Exception("Boleto não encontrado");

                if(!bol.Gerado)
                    throw new Exception("Boleto ainda não gerado");

                var folderpath = Server.MapPath(EdtespConfig.AppDataFolder);
                var path = Path.Combine(folderpath, bol.NomePdf);

                if(!System.IO.File.Exists(path))
                    throw new Exception("Boleto não existe");

                return File(path, "application/pdf", bol.NomePdf);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public JsonResult EnviarBoleto(int boletoId)
        {
            try
            {
                var path = Server.MapPath(EdtespConfig.AppDataFolder);
                _appsvcTit.EnviarPorEmail(path, boletoId);
                return Json(new {error = false}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {error = true, msg = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AlterarVencto(int id, string novoVencto, string valor)
        {
            try
            {
                valor = valor.Replace(".", "");

                var nv = Convert.ToDateTime(novoVencto);
                var total = 0.0M;

                if (!decimal.TryParse(valor, out total))
                    return Json(new { error = true, msg ="Valor inválido" }, JsonRequestBehavior.AllowGet);

                _appsvcTit.AlterarVencto(id, nv, Usuario.UsuarioId, total);

                return Json(new {error = false}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {error = true, msg = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CancelarBoleto(int id, bool novoBoleto, bool semBoleto)
        {
            try
            {
                if (!semBoleto)
                    _appsvcTit.CancelarBoleto(id, Usuario.UsuarioId, novoBoleto);
                else
                    _appsvcTit.CancelarTitulo(id, Usuario.UsuarioId);

                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult BaixarTitulo(int id, string dataBaixa)
        {
            try
            {
                var nv = Convert.ToDateTime(dataBaixa);
                _appsvcTit.BaixarTitulo(id, Usuario.UsuarioId, nv);

                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GerarBoleto(int id)
        {
            try
            {
                _appsvcTit.GerarBoleto(id, Usuario.UsuarioId);
                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteTicket(int id)
        {
            try
            {
                var boleto = _appsvcBol.Get(id);

                if (boleto != null && boleto.BoletoId > 0)
                {
                    var result = _appsvcTit.RemoverBoleto(id);

                    if(result)
                        return Json(new { error = false, msg = "" });
                    else
                        return Json(new { error = true, msg = "Não foi possível remover o boleto" });
                }
                else
                    return Json(new { error = true, msg = "Boleto inválido" });
            }
            catch(Exception ex)
            {
                return Json(new { error = true, msg = ex.Message });
            }
        }

        [HttpGet, FileDownload]
        public FileResult BaixarVariosBoletos(string boletos)
        {
            try
            {
                var listaIds = boletos.Split(',').ToList().Where(b => !string.IsNullOrEmpty(b)).ToList();

                if (listaIds.Any())
                {
                    var lista = new List<string>();

                    foreach (var item in listaIds)
                    {
                        var bol = _appsvcBol.Get(Convert.ToInt32(item));

                        if (bol == null || !bol.Gerado)
                            continue;

                        var folderpath = Server.MapPath(EdtespConfig.AppDataFolder);
                        var path = Path.Combine(folderpath, bol.NomePdf);

                        lista.Add(path);
                    }

                    var zip = new ZipService();

                    var resultado = zip.AddFilesToZip(lista);

                    if (resultado != null && resultado.Length > 0)
                        return File(resultado.ToArray(), "application/zip", "boletos.zip");
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public ActionResult ArquivoRetorno()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ArquivoRetorno(HttpPostedFileBase file)
        {
            try
            {
                var resultadoValidacao = this.ValidarAntesDeProcessarArquivo(file);

                if (resultadoValidacao == "")
                {
                    var retorno =_appsvcTit.ProcessarArquivoRetorno(file.InputStream, Usuario.UsuarioId);

                    if (retorno == "")
                        ViewBag.Sucesso = "Arquivo processado com sucesso";
                    else
                        ViewBag.Erro = retorno;
                }
                else
                    ViewBag.Erro = resultadoValidacao;
            }
            catch (Exception ex)
            {
                ViewBag.Erro = ex.Message;
            }

            return View();
        }

        private string ValidarAntesDeProcessarArquivo(HttpPostedFileBase file)
        {
            var erros = new StringBuilder();

            if (file == null || file.ContentLength == 0)
                erros.Append("Não existe nenhum arquivo para processar. Faça o upload de um arquivo");
            else
            {
                var matrizDados = file.FileName.Split('.');

                if(matrizDados.Length == 0 || matrizDados[matrizDados.Length-1].ToLower() != "ret")
                    erros.Append("Arquivo com extensão inválida");
            }

            return erros.ToString();
        }

        private int RetornarUsuarioParaFiltrar()
        {
            var usuario = 0;

            var dadosUsuario = (Usuario)Session["Usuario"];
            var permissoes = _appsvcPrmsUsr.Where(w => w.UsuarioId == dadosUsuario.UsuarioId).ToList();

            var resultado = permissoes.Where(p => p.Permissao != null && p.Permissao.Role == "GERTITUS").FirstOrDefault();

            if (resultado != null)
                usuario = dadosUsuario.UsuarioId;

            return usuario;
        }

        private bool RetornarPermissaoMarcarComoPago()
        {
            var dadosUsuario = (Usuario)Session["Usuario"];
            var permissoes = _appsvcPrmsUsr.Where(w => w.UsuarioId == dadosUsuario.UsuarioId).ToList();

            var resultado = permissoes.Where(p => p.Permissao != null && p.Permissao.Role == "BXATIT").FirstOrDefault();

            if (resultado != null)
                return true;

            return false;
        }

        private bool RetornarPermissaoMarcarComoNaoPago()
        {
            var dadosUsuario = (Usuario)Session["Usuario"];
            var permissoes = _appsvcPrmsUsr.Where(w => w.UsuarioId == dadosUsuario.UsuarioId).ToList();

            var resultado = permissoes.Where(p => p.Permissao != null && p.Permissao.Role == "NBAIXTIT").FirstOrDefault();

            if (resultado != null)
                return true;

            return false;
        }

        [HttpPost]
        public JsonResult MarcarTituloComoNaoPago(int id)
        {
            try
            {
                var result = _appsvcTit.MarcarComoNaoBaixado(id, Usuario.UsuarioId);

                if (result.First().Key)
                    return Json(new { error = false }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { error = true, msg = result.First().Value }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}