using EDTESP.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EDTESP.Web.ViewModels.Relatorios;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using AutoMapper;
using EDTESP.Domain.Entities.Relatorios;
using System.Net;
using EDTESP.Web.Filters;

namespace EDTESP.Web.Controllers
{
    [RibbonTab(Tab = "relatorios")]
    public class RelatoriosController : BaseController
    {
        private readonly IAppServiceBase<Vendedor> _appServiceBase;
        private readonly IComissaoInvidualAppService _comissaoInvidualAppService;
        private readonly IAdiantamentoSalarialAppService _adiantamentoSalarialAppService;
        private readonly IEstornoComissaoAppService _estornoComissaoAppService;
        private readonly IResumoVendaGeralAppService _resumoVendaGeralAppService;
        private readonly IResumoVendaRepresentanteAppService _resumoVendaRepresentanteAppService;
        private readonly ICobrancaClienteAppService _cobrancaClienteAppService;
        private readonly IAppServiceBase<Cliente> _appServiceBaseCliente;

        public RelatoriosController(IAppServiceBase<Status> appsvcBase, IAppServiceBase<Vendedor> appServiceBase, 
                                    IComissaoInvidualAppService comissaoInvidualAppService, 
                                    IAdiantamentoSalarialAppService adiantamentoSalarialAppService,
                                    IEstornoComissaoAppService estornoComissaoAppService, 
                                    IResumoVendaGeralAppService resumoVendaGeralAppService,
                                    IResumoVendaRepresentanteAppService resumoVendaRepresentanteAppService,
                                    ICobrancaClienteAppService cobrancaClienteAppService,
                                    IAppServiceBase<Cliente> appServiceBaseCliente) : base(appsvcBase)
        {
            _appServiceBase = appServiceBase;
            _comissaoInvidualAppService = comissaoInvidualAppService;
            _adiantamentoSalarialAppService = adiantamentoSalarialAppService;
            _estornoComissaoAppService = estornoComissaoAppService;
            _resumoVendaGeralAppService = resumoVendaGeralAppService;
            _resumoVendaRepresentanteAppService = resumoVendaRepresentanteAppService;
            _cobrancaClienteAppService = cobrancaClienteAppService;
            _appServiceBaseCliente = appServiceBaseCliente;
        }

        [EdtespAuthorize(Roles = "RCOMI")]
        public ActionResult ComissaoIndividual(int p = 1, int pp = 20, int vendedorId = 0, string emiIni = null, string emiFim = null)
        {
            var demiini = string.IsNullOrEmpty(emiIni) ? null : (DateTime?)Convert.ToDateTime(emiIni + " 00:00:00");
            var demifim = string.IsNullOrEmpty(emiFim) ? null : (DateTime?)Convert.ToDateTime(emiFim + " 23:59:59");

            ViewBag.Ini = demiini;
            ViewBag.Fim = demifim;

            var resultado = new List<ComissaoIndividual>();

            if ((demiini != null && demifim != null) || vendedorId > 0)
            {
                resultado = _comissaoInvidualAppService.RetornarDados(vendedorId,  
                                                                      demiini.HasValue? demiini.Value: DateTime.MinValue, 
                                                                      demifim.HasValue? demifim.Value: DateTime.MinValue);
            }

            var resultadoViewModel = Mapper.Map<List<ComissaoIndividual>, List<ComissaoIndividualViewModel>>(resultado);

            ViewBag.vendedorId = this.RetornarVendedores(vendedorId);

            if(resultadoViewModel != null && resultadoViewModel.Any())
            {
                ViewBag.ValorTotalContrato = resultadoViewModel.Sum(s => s.ValorFinal);
                ViewBag.TotalContratos = resultadoViewModel.Count();
                ViewBag.TotalComissao = resultadoViewModel.Sum(s => s.ValorReceber);
            }

            ViewBag.TituloPdf = "Perído " + demiini + " à " + demifim;

            return View("ComissaoIndividual", resultadoViewModel);
        }

        private SelectList RetornarVendedores(int vendedorId)
        {
            var lista = _appServiceBase.List().OrderBy(v => v.Nome).ToList();

            lista.Add(new Vendedor() { VendedorId = 0, Nome = "Selecione..." });

            return new SelectList
           (
               lista,
               "VendedorId",
               "Nome",
               vendedorId
           );
        }

        [EdtespAuthorize(Roles = "RADS")]
        public ActionResult AdiantamentoSalarial(int p = 1, int pp = 20, int vendedorId = 0, string emiIni = null, string emiFim = null)
        {
            var demiini = string.IsNullOrEmpty(emiIni) ? null : (DateTime?)Convert.ToDateTime(emiIni + " 00:00:00");
            var demifim = string.IsNullOrEmpty(emiFim) ? null : (DateTime?)Convert.ToDateTime(emiFim + " 23:59:59");

            ViewBag.Ini = demiini;
            ViewBag.Fim = demifim;

            var resultado = new List<AdiantamentoSalarial>();

            if ((demiini != null && demifim != null) || vendedorId > 0)
            {
                resultado = _adiantamentoSalarialAppService.RetornarDados(vendedorId,
                                                                      demiini.HasValue ? demiini.Value : DateTime.MinValue,
                                                                      demifim.HasValue ? demifim.Value : DateTime.MinValue);
            }

            var resultadoViewModel = Mapper.Map<List<AdiantamentoSalarial>, List<AdiantamentoSalarialViewModel>>(resultado);

            ViewBag.vendedorId = this.RetornarVendedores(vendedorId);

            if(resultadoViewModel != null && resultadoViewModel.Any())
            {
                ViewBag.QuantidadeContrato = resultadoViewModel.Count();
                ViewBag.ValorTotalContrato = resultadoViewModel.Sum(s => s.ValorBase);
                ViewBag.ValorTotalComissao = resultadoViewModel.Sum(s => s.ValorReceber);
                ViewBag.ValorTotalAdiantamento = resultadoViewModel.Sum(s => s.ValorAdiantamento);
            }

            return View("AdiantamentoSalarial", resultadoViewModel);
        }

        [EdtespAuthorize(Roles = "RESTC")]
        public ActionResult EstornoComissao(int p = 1, int pp = 20, int vendedorId = 0, string emiIni = null, string emiFim = null)
        {
            var demiini = string.IsNullOrEmpty(emiIni) ? null : (DateTime?)Convert.ToDateTime(emiIni + " 00:00:00");
            var demifim = string.IsNullOrEmpty(emiFim) ? null : (DateTime?)Convert.ToDateTime(emiFim + " 23:59:59");

            ViewBag.Ini = demiini;
            ViewBag.Fim = demifim;

            var resultado = new List<EstornoComissao>();

            if ((demiini != null && demifim != null) || vendedorId > 0)
            {
                resultado = _estornoComissaoAppService.RetornarDados(vendedorId,
                                                                      demiini.HasValue ? demiini.Value : DateTime.MinValue,
                                                                      demifim.HasValue ? demifim.Value : DateTime.MinValue,
                                                                      DateTime.Now.Date);
            }

            var resultadoViewModel = Mapper.Map<List<EstornoComissao>, List<EstornoComissaoViewModel>>(resultado);

            ViewBag.vendedorId = this.RetornarVendedores(vendedorId);

            if (resultadoViewModel != null && resultadoViewModel.Any())
            {
                ViewBag.ValorTotalContrato = resultadoViewModel.Sum(s => s.ValorFinal);
                ViewBag.ValorTotalComissao = resultadoViewModel.Sum(s => s.ValorReceber);
                ViewBag.ValorTotalParcela = resultadoViewModel.Sum(s => s.ValorParcela);
            }

            return View("EstornoComissao", resultadoViewModel);
        }

        [EdtespAuthorize(Roles = "RRVG")]
        public ActionResult ResumoVendaGeral(int p = 1, int pp = 20, int vendedorId = 0, string emiIni = null, string emiFim = null)
        {
            var demiini = string.IsNullOrEmpty(emiIni) ? null : (DateTime?)Convert.ToDateTime(emiIni + " 00:00:00");
            var demifim = string.IsNullOrEmpty(emiFim) ? null : (DateTime?)Convert.ToDateTime(emiFim + " 23:59:59");

            ViewBag.Ini = demiini;
            ViewBag.Fim = demifim;

            var resultado = new List<ResumoVendaGeral>();

            if ((demiini != null && demifim != null) || vendedorId > 0)
            {
                resultado = _resumoVendaGeralAppService.RetornarDados(vendedorId,
                                                                      demiini.HasValue ? demiini.Value : DateTime.MinValue,
                                                                      demifim.HasValue ? demifim.Value : DateTime.MinValue);
            }

            var resultadoViewModel = Mapper.Map<List<ResumoVendaGeral>, List<ResumoVendaGeralViewModel>>(resultado);

            ViewBag.vendedorId = this.RetornarVendedores(vendedorId);

            if (resultadoViewModel != null && resultadoViewModel.Any())
                ViewBag.ValorTotalContrato = resultadoViewModel.Sum(s => s.ValorFinal);

            return View("ResumoVendaGeral", resultadoViewModel);
        }

        [EdtespAuthorize(Roles = "RRESV")]
        public ActionResult ResumoVendaRepresentante(int p = 1, int pp = 20, int vendedorId = 0, string emiIni = null, string emiFim = null)
        {
            var demiini = string.IsNullOrEmpty(emiIni) ? null : (DateTime?)Convert.ToDateTime(emiIni + " 00:00:00");
            var demifim = string.IsNullOrEmpty(emiFim) ? null : (DateTime?)Convert.ToDateTime(emiFim + " 23:59:59");

            ViewBag.Ini = demiini;
            ViewBag.Fim = demifim;

            var resultado = new List<ResumoVendaRepresentante>();

            if ((demiini != null && demifim != null) || vendedorId > 0)
            {
                resultado = _resumoVendaRepresentanteAppService.RetornarDados(vendedorId,
                                                                              demiini.HasValue ? demiini.Value : DateTime.MinValue,
                                                                              demifim.HasValue ? demifim.Value : DateTime.MinValue);
            }

            var resultadoViewModel = Mapper.Map<List<ResumoVendaRepresentante>, List<ResumoVendaRepresentanteViewModel>>(resultado);

            ViewBag.vendedorId = this.RetornarVendedores(vendedorId);

            if (resultadoViewModel != null && resultadoViewModel.Any())
            {
                ViewBag.ValorTotalContrato = resultadoViewModel.Sum(s => s.ValorFinal);
                ViewBag.ValorTotalComissao = resultadoViewModel.Sum(s => s.ValorReceber);
            }

            return View("ResumoVendaRepresentante", resultadoViewModel);
        }

        [EdtespAuthorize(Roles = "RCOB")]
        public ActionResult CobrancaCliente(int p = 1, int pp = 20, int clienteId = 0, string emiIni = null, string emiFim = null, int vendedorId = 0)
        {
            var demiini = string.IsNullOrEmpty(emiIni) ? null : (DateTime?)Convert.ToDateTime(emiIni + " 00:00:00");
            var demifim = string.IsNullOrEmpty(emiFim) ? null : (DateTime?)Convert.ToDateTime(emiFim + " 23:59:59");

            ViewBag.Ini = demiini;
            ViewBag.Fim = demifim;

            var resultado = new List<CobrancaCliente>();

            if ((demiini != null && demifim != null) || clienteId > 0)
            {
                resultado = _cobrancaClienteAppService.RetornarDados(clienteId,
                                                                     demiini.HasValue ? demiini.Value : DateTime.MinValue,
                                                                     demifim.HasValue ? demifim.Value : DateTime.MinValue,
                                                                     DateTime.Now,
                                                                     vendedorId);
            }

            var resultadoViewModel = Mapper.Map<List<CobrancaCliente>, List<CobrancaClienteViewModel>>(resultado);

            ViewBag.clienteId = this.RetornarCliente(clienteId);

            ViewBag.vendedorId = this.RetornarVendedores(vendedorId);

            return View("CobrancaCliente", resultadoViewModel);
        }

        [HttpPost]
        public FileResult ExportarExcel(string cliente, string emisIni, string emisFim, string vendedor)
        {
            var demiini = string.IsNullOrEmpty(emisIni) ? null : (DateTime?)Convert.ToDateTime(emisIni + " 00:00:00");
            var demifim = string.IsNullOrEmpty(emisFim) ? null : (DateTime?)Convert.ToDateTime(emisFim + " 23:59:59");

            var resultado = _cobrancaClienteAppService.GerarExcel(Convert.ToInt32(cliente),
                                                                     demiini.HasValue ? demiini.Value : DateTime.MinValue,
                                                                     demifim.HasValue ? demifim.Value : DateTime.MinValue,
                                                                     DateTime.Now,
                                                                     Convert.ToInt32(vendedor));

            return File(resultado.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CobrancaCliente.xlsx");
        }

        private SelectList RetornarCliente(int clienteId)
        {
            var lista = _appServiceBaseCliente.List().OrderBy(v => v.RazaoSocial).ToList();

            lista.Add(new Cliente() { ClienteId = 0, RazaoSocial = "Selecione..." });

            return new SelectList
           (
               lista,
               "ClienteId",
               "RazaoSocial",
               clienteId
           );
        }

        [AllowAnonymous]
        [HttpGet, FileDownload]
        public FileResult RetornarPdfCobrancaClienteIndividual(string cliente, string emisIni, string emisFim, string vendedor)
        {
            try
            {
                var demiini = string.IsNullOrEmpty(emisIni) ? null : (DateTime?)Convert.ToDateTime(emisIni + " 00:00:00");
                var demifim = string.IsNullOrEmpty(emisFim) ? null : (DateTime?)Convert.ToDateTime(emisFim + " 23:59:59");

                var pdf = _cobrancaClienteAppService.RetornarDadosPdf(Convert.ToInt32(cliente), demiini.Value, demifim.Value, DateTime.Now, Convert.ToInt32(vendedor));

                return File(pdf, "application/pdf", "cobrancaIndividual.pdf");
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                return null;
            }
        }
    }
}