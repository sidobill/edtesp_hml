using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using AutoMapper;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Infrastructure.CC.Util;
using EDTESP.Web.Filters;
using EDTESP.Web.Helpers;
using EDTESP.Web.ViewModels;
using EDTESP.Web.ViewModels.Cadastros;
using EDTESP.Web.ViewModels.Operacoes;
using Newtonsoft.Json;
using PagedList;

namespace EDTESP.Web.Controllers.Operacoes
{
    [RibbonTab(Tab = "operacao")]
    public class ContratosController : BaseController
    {
        private readonly IContratoAppService _appsvcContr;
        private readonly IAppServiceBase<Cliente> _appsvcCli;
        private readonly IAppServiceBase<Vendedor> _appsvcVnd;
        private readonly IAppServiceBase<FormaPagamento> _appsvcForma;
        private readonly ICondicaoPagamentoAppService _appsvcCond;
        private readonly IAppServiceBase<StatusContrato> _appsvcStt;
        private readonly IAppServiceBase<Produto> _appsvcPrd;
        private readonly IAppServiceBase<Empresa> _appsvcEmp;
        private readonly ITituloAppService _appsvcTit;
        private readonly IAppServiceBase<MotivoSuspensao> _appsvcMot;
        private readonly IAppServiceBase<PermissaoUsuario> _appsvcPrmsUsr;
        private readonly IAppServiceBase<ContratoEvento> _appContrEvent;

        public ContratosController(IAppServiceBase<Status> appsvcBase,
            IContratoAppService appsvcContr,
            IAppServiceBase<Cliente> appsvcCli,
            IAppServiceBase<Vendedor> appsvcVnd,
            IAppServiceBase<FormaPagamento> appsvcForma,
            ICondicaoPagamentoAppService appsvcCond,
            IAppServiceBase<StatusContrato> appsvcStt,
            IAppServiceBase<Produto> appsvcPrd,
            IAppServiceBase<Empresa> appsvcEmp,
            ITituloAppService appsvcTit,
            IAppServiceBase<MotivoSuspensao> appsvcMot,
            IAppServiceBase<PermissaoUsuario> appsvcPrmsUsr,
            IAppServiceBase<ContratoEvento> appContrEvent
        ) : base(appsvcBase)
        {
            _appsvcContr = appsvcContr;
            _appsvcCli = appsvcCli;
            _appsvcVnd = appsvcVnd;
            _appsvcForma = appsvcForma;
            _appsvcCond = appsvcCond;
            _appsvcStt = appsvcStt;
            _appsvcPrd = appsvcPrd;
            _appsvcEmp = appsvcEmp;
            _appsvcTit = appsvcTit;
            _appsvcMot = appsvcMot;
            _appsvcPrmsUsr = appsvcPrmsUsr;
            _appContrEvent = appContrEvent;
        }

        //private void ListarFormas()
        //{
        //    var lst = _appsvcForma.Where(x => !x.Removido).ToList().Select(x => new SelectListItem
        //    {
        //        Value = x.FormaPagamentoId.ToString(),
        //        Text = x.Descricao
        //    }).ToList();

        //    lst.Insert(0, new SelectListItem
        //    {
        //        Value = null,
        //        Text = "Selecione",
        //        Disabled = true
        //    });
        //    ViewBag.Formas = lst;
        //}

        private void ListarFormas()
        {
            //var forms = _appsvcForma.Where(x => !x.Removido).ToList();
            var conds = _appsvcCond.Where(x => !x.Removido).ToList();
            var forms = conds.SelectMany(x => x.FormasPagamentos).Distinct().ToList();

            var opts = new List<SelectListItem>();
            foreach (var forma in forms)
            {
                var optg = new SelectListGroup
                {
                    Name = forma.Descricao,
                };

                opts.AddRange(conds
                    .Where(x => x.FormasPagamentos.Any(y => y.FormaPagamentoId == forma.FormaPagamentoId)).Select(x =>
                        new SelectListItem
                        {
                            Group = optg,
                            Value = $"{forma.FormaPagamentoId}:{x.CondicaoPagamentoId}",
                            Text = x.Descricao
                        }));
            }

            opts.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Selecione",
                Disabled = true,
            });


            ViewBag.Formas = opts;
        }

        private void ListarFormas1Parc()
        {
            //var forms = _appsvcForma.Where(x => !x.Removido).ToList();
            var conds = _appsvcCond.Where(x => !x.Removido && x.Usado1Parc).ToList();
            var forms = conds.SelectMany(x => x.FormasPagamentos).Distinct().ToList();

            var opts = new List<SelectListItem>();
            foreach (var forma in forms)
            {
                var optg = new SelectListGroup
                {
                    Name = forma.Descricao,
                };

                opts.AddRange(conds
                    .Where(x => x.FormasPagamentos.Any(y => y.FormaPagamentoId == forma.FormaPagamentoId)).Select(x =>
                        new SelectListItem
                        {
                            Group = optg,
                            Value = $"{forma.FormaPagamentoId}:{x.CondicaoPagamentoId}",
                            Text = x.Descricao
                        }));
            }

            opts.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Selecione",
                Disabled = true,
            });


            ViewBag.Formas1Parc = opts;
        }

        private void ListarCedentes()
        {
            var lst = _appsvcEmp.List().ToList().Select(x => new SelectListItem
            {
                Value = x.EmpresaId.ToString(),
                Text = x.RazaoSocial
            }).ToList();
            lst.Insert(0, new SelectListItem
            {
                Value = null,
                Text = "Selecione",
                Disabled = true
            });
            ViewBag.Empresas = lst;
        }
        
        public ActionResult Index()
        {
            return View();
        }

        [EdtespAuthorize(Roles = "GERCTR,VPCON")]
        public ActionResult EmAnalise(int p = 1, int pp = 20, string termo = null, string emiIni = null, string emiFim = null)
        {
            var ntermo = termo.ClearNumber();

            int itermo = 0;
            if (!string.IsNullOrEmpty(ntermo))
                int.TryParse(ntermo, out itermo);

            var demiini = string.IsNullOrEmpty(emiIni) ? null : (DateTime?)Convert.ToDateTime(emiIni + " 00:00:00");
            var demifim = string.IsNullOrEmpty(emiFim) ? null : (DateTime?)Convert.ToDateTime(emiFim + " 23:59:59");

            ViewBag.Ini = demiini;
            ViewBag.Fim = demifim;

            var usuarioFiltrar = RetornarUsuarioParaFiltrar();

            List<Contrato> list = null;

            if (usuarioFiltrar == 0)
            {
                list = _appsvcContr.Where(x => x.StatusContratoId == (int)EnumStatusContrato.EmAnalise &&
                                           (string.IsNullOrEmpty(termo)
                                            || x.Numero.Equals(itermo)
                                            || x.Cliente.RazaoSocial.ToLower()
                                                .Contains(termo.ToLower())
                                            || x.Cliente.Fantasia.ToLower()
                                                .Contains(termo.ToLower())
                                            || x.Descricao.ToLower()
                                                .Contains(termo.ToLower())
                                            || x.Categoria.ToLower()
                                                .Contains(termo.ToLower())
                                            || x.Especie.ToLower()
                                                .Contains(termo.ToLower())
                                            || (!string.IsNullOrEmpty(ntermo) && (
                                                    x.Cliente.Documento.Contains(ntermo)
                                                    || x.Vendedor.Cpf.Contains(ntermo)
                                                )
                                            )
                                            && (!demiini.HasValue || (x.DataCriacao >= demiini && x.DataCriacao <= demifim))
                                            )).ToList();
            }
            else
            {
                list = _appsvcContr.Where(x => x.StatusContratoId == (int)EnumStatusContrato.EmAnalise && x.UsuarioCriadorId == usuarioFiltrar &&
                                           (string.IsNullOrEmpty(termo)
                                            || x.Numero.Equals(itermo)
                                            || x.Cliente.RazaoSocial.ToLower()
                                                .Contains(termo.ToLower())
                                            || x.Cliente.Fantasia.ToLower()
                                                .Contains(termo.ToLower())
                                            || x.Descricao.ToLower()
                                                .Contains(termo.ToLower())
                                            || x.Categoria.ToLower()
                                                .Contains(termo.ToLower())
                                            || x.Especie.ToLower()
                                                .Contains(termo.ToLower())
                                            || (!string.IsNullOrEmpty(ntermo) && (
                                                    x.Cliente.Documento.Contains(ntermo)
                                                    || x.Vendedor.Cpf.Contains(ntermo)
                                                )
                                            )
                                            && (!demiini.HasValue || (x.DataCriacao >= demiini && x.DataCriacao <= demifim))
                                            )).ToList();
            }

            var lst = Mapper.Map<List<ContratoViewModel>>(list);

            if (lst != null && lst.Any())
                lst = this.ColocarPermissoesBotoes(lst);

            var model = CriarPaginador(lst, p, pp, termo);
            ViewBag.Status = "Em Análise";
            return View("_Index", model);
        }

        [EdtespAuthorize(Roles = "GERCTR,VPCON")]
        public ActionResult Aprovados(int p = 1, int pp = 20, string termo = null, string emiIni = null, string emiFim = null)
        {
            var list = new List<Contrato>();

            if (!string.IsNullOrEmpty(termo) || !string.IsNullOrEmpty(emiIni))
            {
                var ntermo = termo.ClearNumber();

                var itermo = 0;
                if (!string.IsNullOrEmpty(ntermo))
                    int.TryParse(ntermo, out itermo);

                var demiini = string.IsNullOrEmpty(emiIni) ? null : (DateTime?)Convert.ToDateTime(emiIni + " 00:00:00");
                var demifim = string.IsNullOrEmpty(emiFim) ? null : (DateTime?)Convert.ToDateTime(emiFim + " 23:59:59");

                ViewBag.Ini = demiini;
                ViewBag.Fim = demifim;

                var usuarioFiltrar = RetornarUsuarioParaFiltrar();

                if (usuarioFiltrar == 0)
                {
                    list = _appsvcContr.Where(x => x.StatusContratoId == 1 && (string.IsNullOrEmpty(termo)
                                                                               || x.Numero.Equals(itermo)
                                                                               || x.Cliente.RazaoSocial.ToLower()
                                                                                   .Contains(termo.ToLower())
                                                                               || x.Cliente.Fantasia.ToLower()
                                                                                   .Contains(termo.ToLower())
                                                                               || x.Descricao.ToLower()
                                                                                   .Contains(termo.ToLower())
                                                                               || x.Categoria.ToLower()
                                                                                   .Contains(termo.ToLower())
                                                                               || x.Especie.ToLower()
                                                                                   .Contains(termo.ToLower())
                                                                               || (!string.IsNullOrEmpty(ntermo) && (
                                                                                          x.Cliente.Documento.Contains(ntermo)
                                                                                       || x.Vendedor.Cpf.Contains(ntermo)
                                                                                   )
                                                                               ))
                                                   && (!demiini.HasValue || (x.DataCriacao >= demiini && x.DataCriacao <= demifim))
                                                   ).ToList();
                }
                else
                {
                    list = _appsvcContr.Where(x => x.StatusContratoId == 1 && x.UsuarioCriadorId == usuarioFiltrar && (string.IsNullOrEmpty(termo)
                                                                               || x.Numero.Equals(itermo)
                                                                               || x.Cliente.RazaoSocial.ToLower()
                                                                                   .Contains(termo.ToLower())
                                                                               || x.Cliente.Fantasia.ToLower()
                                                                                   .Contains(termo.ToLower())
                                                                               || x.Descricao.ToLower()
                                                                                   .Contains(termo.ToLower())
                                                                               || x.Categoria.ToLower()
                                                                                   .Contains(termo.ToLower())
                                                                               || x.Especie.ToLower()
                                                                                   .Contains(termo.ToLower())
                                                                               || (!string.IsNullOrEmpty(ntermo) && (
                                                                                          x.Cliente.Documento.Contains(ntermo)
                                                                                       || x.Vendedor.Cpf.Contains(ntermo)
                                                                                   )
                                                                               ))
                                                   && (!demiini.HasValue || (x.DataCriacao >= demiini && x.DataCriacao <= demifim))
                                                   ).ToList();
                }
            }

            var lst = Mapper.Map<List<ContratoViewModel>>(list);

            if (lst != null && lst.Any())
                lst = this.ColocarPermissoesBotoes(lst);

            var model = CriarPaginador(lst, p, pp, termo);
            ViewBag.Status = "Aprovados";

            return View("_Index", model);
        }

        [EdtespAuthorize(Roles = "GERCTR")]
        public ActionResult Reprovados(int p = 1, int pp = 20, string termo = null, string emiIni = null, string emiFim = null)
        {
            var list = new List<Contrato>();

            if (!string.IsNullOrEmpty(termo) || !string.IsNullOrEmpty(emiIni))
            {
                var ntermo = termo.ClearNumber();

                var itermo = 0;
                if (!string.IsNullOrEmpty(ntermo))
                    int.TryParse(ntermo, out itermo);

                var demiini = string.IsNullOrEmpty(emiIni) ? null : (DateTime?)Convert.ToDateTime(emiIni+" 00:00:00");
                var demifim = string.IsNullOrEmpty(emiFim) ? null : (DateTime?)Convert.ToDateTime(emiFim + " 23:59:59");

                ViewBag.Ini = demiini;
                ViewBag.Fim = demifim;

                list = _appsvcContr.Where(x => x.StatusContratoId == (int) EnumStatusContrato.Reprovado &&
                                               (string.IsNullOrEmpty(termo)
                                                || x.Numero.Equals(itermo)
                                                || x.Cliente.RazaoSocial.ToLower().Contains(termo.ToLower())
                                                || x.Cliente.Fantasia.ToLower().Contains(termo.ToLower())
                                                || x.Descricao.ToLower().Contains(termo.ToLower())
                                                || x.Categoria.ToLower().Contains(termo.ToLower())
                                                || x.Especie.ToLower().Contains(termo.ToLower())
                                                || (!string.IsNullOrEmpty(ntermo) && (
                                                        x.Cliente.Documento.Contains(ntermo)
                                                        || x.Vendedor.Cpf.Contains(ntermo)
                                                    )
                                                ))
                                               && (!demiini.HasValue || (x.DataCriacao >= demiini && x.DataCriacao <= demifim))
                                               ).ToList();
            }

            var lst = Mapper.Map<List<ContratoViewModel>>(list);

            if (lst != null && lst.Any())
                lst = this.ColocarPermissoesBotoes(lst);

            var model = CriarPaginador(lst, p, pp, termo);
            ViewBag.Status = "Reprovados";
            return View("_Index", model);
        }

        [EdtespAuthorize(Roles = "GERCTR")]
        public ActionResult Suspensos(int p = 1, int pp = 20, string termo = null, string emiIni = null, string emiFim = null)
        {
            var list = new List<Contrato>();

            if (!string.IsNullOrEmpty(termo) || !string.IsNullOrEmpty(emiIni))
            {
                var ntermo = termo.ClearNumber();

                var itermo = 0;
                if (!string.IsNullOrEmpty(ntermo))
                    int.TryParse(ntermo, out itermo);

                var demiini = string.IsNullOrEmpty(emiIni) ? null : (DateTime?)Convert.ToDateTime(emiIni + " 00:00:00");
                var demifim = string.IsNullOrEmpty(emiFim) ? null : (DateTime?)Convert.ToDateTime(emiFim + " 23:59:59");

                ViewBag.Ini = demiini;
                ViewBag.Fim = demifim;

                list = _appsvcContr.Where(x => x.StatusContratoId == (int)EnumStatusContrato.Suspenso &&
                                               (string.IsNullOrEmpty(termo)
                                                || x.Numero.Equals(itermo)
                                                || x.Cliente.RazaoSocial.ToLower().Contains(termo.ToLower())
                                                || x.Cliente.Fantasia.ToLower().Contains(termo.ToLower())
                                                || x.Descricao.ToLower().Contains(termo.ToLower())
                                                || x.Categoria.ToLower().Contains(termo.ToLower())
                                                || x.Especie.ToLower().Contains(termo.ToLower())
                                                || (!string.IsNullOrEmpty(ntermo) && (
                                                        x.Cliente.Documento.Contains(ntermo)
                                                        || x.Vendedor.Cpf.Contains(ntermo)
                                                    )
                                                ))
                                               && (!demiini.HasValue || (x.DataCriacao >= demiini && x.DataCriacao <= demifim))
                                               ).ToList();
            }

            var lst = Mapper.Map<List<ContratoViewModel>>(list);

            if (lst != null && lst.Any())
                lst = this.ColocarPermissoesBotoes(lst);

            var model = CriarPaginador(lst, p, pp, termo);
            ViewBag.Status = "Suspensos";
            return View("_Index", model);
        }
        
        [EdtespAuthorize(Roles = "GERCTR")]
        public ActionResult Ver(int contratoId)
        {
            try
            {
                var ctr = _appsvcContr.Get(contratoId);

                if (ctr == null)
                    throw new Exception("Contrato não encontrado");

                ctr.Titulos = ctr.Titulos.Where(x => !x.Removido).ToList();
                ctr.ContratoEventos = ctr.ContratoEventos.OrderByDescending(x => x.DataCriacao).ToList();
                var model = Mapper.Map<ContratoViewModel>(ctr);

                return View(model);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [EdtespAuthorize(Roles = "MODCTR")]
        public ActionResult Editar(int id)
        {
            var ctr = _appsvcContr.Get(id);

            if (ctr.StatusContratoId != (int) EnumStatusContrato.EmAnalise)
                return RedirectToAction("EmAnalise", "Contratos");

            var model = Mapper.Map<ContratoViewModel>(ctr);
            model.CondicaoPagamentoIdStr = $"{ctr.FormaPagamentoId}:{ctr.CondicaoPagamentoId}";
            model.CondicaoPagamento1ParcIdStr = $"{ctr.FormaPagamento1ParcId}:{ctr.CondicaoPagamento1ParcId}";

            ListarCedentes();
            ListarFormas();
            ListarFormas1Parc();
            return View(model);
        }

        [EdtespAuthorize(Roles = "MODCTR")]
        public ActionResult Reaprovar(int id)
        {
            var ctr = _appsvcContr.Get(id);

            if (ctr.StatusContratoId != (int)EnumStatusContrato.Suspenso)
                return RedirectToAction("Suspensos", "Contratos");

            var model = Mapper.Map<ContratoViewModel>(ctr);
            model.ValorBase = ctr.Titulos.Where(x => !x.Removido && x.Suspenso && !x.DataBaixa.HasValue)
                .Sum(x => x.Valor);

            model.DataVencto1Parc = DateTime.Now.AddDays(1);
            model.ValorFinal = model.ValorBase - model.Desconto + model.Tarifas;
            model.CondicaoPagamentoIdStr = $"{ctr.FormaPagamentoId}:{ctr.CondicaoPagamentoId}";
            model.CondicaoPagamento1ParcIdStr = $"{ctr.FormaPagamento1ParcId}:{ctr.CondicaoPagamento1ParcId}";

            ListarCedentes();
            ListarFormas();
            ListarFormas1Parc();
            return View(model);
        }

        [HttpPost]
        [EdtespAuthorize(Roles = "EDINCON")]
        public ActionResult Editar(ContratoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifque os erros a seguir");

                if(model.PrimeiraParcela <= 0)
                    throw new Exception("Informe o valor da 1° parcela");

                var ctr = _appsvcContr.Get(model.ContratoId);
                Mapper.Map(model, ctr);

                int fp, cp;
                var str = model.CondicaoPagamentoIdStr.Split(':');

                if (str.Length != 2)
                    throw new Exception("Condição de pgto. inválida");

                int.TryParse(str[0], out fp);
                int.TryParse(str[1], out cp);

                ctr.FormaPagamentoId = fp;
                ctr.CondicaoPagamentoId = cp;

                ctr.UsuarioAtualizadorId = Usuario.UsuarioId;
                ctr.DataAtualizacao = DateTime.Now;

                _appsvcContr.Update(ctr);
                return RedirectToAction("EmAnalise", "Contratos");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                ListarCedentes();
                ListarFormas();
                ListarFormas1Parc();
                return View(model);
            }
        }

        [HttpPost]
        [EdtespAuthorize(Roles = "MODCTR")]
        public ActionResult Reaprovar(ContratoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifque os erros a seguir");

                if (model.PrimeiraParcela <= 0)
                    throw new Exception("Informe o valor da 1° parcela");

                var obj = _appsvcContr.Get(model.ContratoId);

                int fp, cp, fp1, cp1;
                var str = model.CondicaoPagamentoIdStr.Split(':');
                var str1p = model.CondicaoPagamento1ParcIdStr.Split(':');

                if (str.Length != 2)
                    throw new Exception("Condição de pgto. inválida");

                if (str1p.Length != 2)
                    throw new Exception("Condição de pgto. 1a parc. inválida");

                int.TryParse(str[0], out fp);
                int.TryParse(str[1], out cp);

                int.TryParse(str1p[0], out fp1);
                int.TryParse(str1p[1], out cp1);

                obj.FormaPagamentoId = fp;
                obj.CondicaoPagamentoId = cp;
                obj.FormaPagamento1ParcId = fp1;
                obj.CondicaoPagamento1ParcId = cp1;

                obj.DataVencto1Parc = model.DataVencto1Parc;
                obj.ValorBase = model.ValorBase;
                obj.PrimeiraParcela = model.PrimeiraParcela;
                obj.Tarifas = model.Tarifas;
                obj.Desconto = model.Desconto;
                obj.ValorFinal = obj.ValorBase + obj.Tarifas - obj.Desconto;
                obj.Categoria = model.Categoria;
                obj.Especie = model.Especie;
                obj.Descricao = model.Descricao;
                obj.Observacao = model.Observacao;

                obj.UsuarioAtualizadorId = Usuario.UsuarioId;
                obj.DataAtualizacao = DateTime.Now;

                _appsvcContr.Reaprovar(obj);
                return RedirectToAction("Aprovados", "Contratos", new {termo = obj.Numero});
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                ListarCedentes();
                ListarFormas();
                ListarFormas1Parc();
                return View(model);
            }
        }

        [EdtespAuthorize(Roles = "MODCTR")]
        public ActionResult Novo()
        {
            var model = new ContratoViewModel
            {
                Numero = null,
                AnoEdicao = DateTime.Now.Year,
                DataInicio = DateTime.Now.AddDays(1),
                DataTermino = DateTime.Now.AddYears(1),
                DataVencto1Parc = DateTime.Now.AddDays(1),
                ValorBase = 0,
                Tarifas = 0,
                Desconto = 0,
                ValorFinal = 0,
                CondicaoPagamentoIdStr = string.Empty,
                CondicaoPagamento1ParcIdStr = string.Empty
            };

            ListarCedentes();
            ListarFormas();
            ListarFormas1Parc();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EdtespAuthorize(Roles = "EDINCON")]
        public ActionResult Novo(ContratoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Verifique os erros a seguir");

                //if (model.PrimeiraParcela <= 0)
                //    throw new Exception("Informe o valor da 1° parcela");

                var obj = Mapper.Map<Contrato>(model);

                int fp, cp, fp1, cp1;
                var str = model.CondicaoPagamentoIdStr.Split(':');

                if (str.Length != 2)
                    throw new Exception("Condição de pgto. inválida");

                //if (str1p.Length != 2)
                //    throw new Exception("Condição de pgto. 1a parc. inválida");



                int.TryParse(str[0], out fp);
                int.TryParse(str[1], out cp);

                if (!string.IsNullOrEmpty(model.CondicaoPagamento1ParcIdStr))
                {
                    var str1p = model.CondicaoPagamento1ParcIdStr.Split(':');

                    if (str1p.Length != 2)
                        throw new Exception("Condição de pgto. 1a parc. inválida");

                    int.TryParse(str1p[0], out fp1);
                    int.TryParse(str1p[1], out cp1);
                }
                else
                {
                    fp1 = fp;
                    cp1 = cp;
                }

                obj.FormaPagamentoId = fp;
                obj.CondicaoPagamentoId = cp;
                obj.FormaPagamento1ParcId = fp1;
                obj.CondicaoPagamento1ParcId = cp1;
                obj.UsuarioCriadorId = Usuario.UsuarioId;
                obj.ValorFinal = obj.ValorBase + obj.Tarifas - obj.Desconto;
                obj.StatusContratoId = (int) EnumStatusContrato.EmAnalise;

                _appsvcContr.Insert(obj);

                return RedirectToAction("EmAnalise", "Contratos");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);

                ListarCedentes();
                ListarFormas();
                ListarFormas1Parc();
                return View(model);
            }
        }

        [HttpPost]
        [EdtespAuthorize(Roles = "APVCTR")]
        public JsonResult AprovarContrato(int id, bool enviaEmail = false)
        {
            try
            {
                var contr = _appsvcContr.Get(id);

                if (contr == null)
                    return Json(new {error = true, msg = "Contrato não encontrado"}, JsonRequestBehavior.AllowGet);

                contr.UsuarioAtualizadorId = Usuario.UsuarioId;
                contr.DataAtualizacao = DateTime.Now;
                _appsvcContr.Aprovar(contr);

                if (enviaEmail)
                {
                    var pasta = Server.MapPath("~/TemplateRdlc/");

                    _appsvcContr.EnviarContratoEmail(id, pasta);
                }

                var uh = new UrlHelper(ControllerContext.RequestContext);
                var url = uh.Action("ViaContrato", "Contratos", new {contratoId = id}, Request.Url.Scheme);

                return Json(new {error = false, url}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {error = true, msg = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [EdtespAuthorize(Roles = "APVCTR")]
        public JsonResult ReprovarContrato(int id)
        {
            try
            {
                var contr = _appsvcContr.Get(id);

                if (contr == null)
                    return Json(new {error = true, msg = "Contrato não encontrado"}, JsonRequestBehavior.AllowGet);

                contr.UsuarioAtualizadorId = Usuario.UsuarioId;
                contr.DataAtualizacao = DateTime.Now;
                _appsvcContr.Reprovar(contr);

                return Json(new {error = false}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {error = true, msg = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [EdtespAuthorize(Roles = "SUSCTR")]
        public JsonResult Suspender(int contratoId, int motivoId, string obs)
        {
            try
            {
                var contr = _appsvcContr.Get(contratoId);

                if(contr == null)
                    throw new Exception("Contrato não existe");

                _appsvcContr.Suspender(contr, motivoId, obs, Usuario.UsuarioId);

                return Json(new {error = false}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {error = true, msg = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EnviarBoletos(int id)
        {
            try
            {
                var path = Server.MapPath(EdtespConfig.AppDataFolder);
                _appsvcTit.EnviarBoletosContrato(id, path);
                return Json(new {error = false}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {error = true, msg = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EnviarContrato(int id)
        {
            try
            {
                _appsvcContr.EnviarContratoEmail(id);

                var contrato = _appsvcContr.Get(id);

                if (contrato != null)
                {
                    _appContrEvent.Insert(new ContratoEvento()
                    {
                        ContratoId = id,
                        DataCriacao = DateTime.Now,
                        Observacao = "Envio por e-mail do recibo",
                        StatusContratoId = contrato.StatusContratoId,
                        UsuarioCriadorId = Usuario.UsuarioId
                    });
                }

                return Json(new {error = false}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {error = true, msg = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        [HttpGet, FileDownload]
        public FileResult ViaContrato(int contratoId)
        {
            try
            {
                var pastaTemplate = Server.MapPath("~/TemplateRdlc/");

                var via = _appsvcContr.GerarViaContrato(contratoId, pastaTemplate);

                var contrato = _appsvcContr.Get(contratoId);

                if (contrato != null)
                {
                    _appContrEvent.Insert(new ContratoEvento()
                    {
                        ContratoId = contratoId,
                        DataCriacao = DateTime.Now,
                        Observacao = "Dowload do recibo",
                        StatusContratoId = contrato.StatusContratoId,
                        UsuarioCriadorId = Usuario.UsuarioId
                    });
                }

                return File(via, "application/pdf", "contrato.pdf");
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                Exception exc = ex;
                do
                {
                    Response.StatusDescription += exc.Message + @"\r\n";
                    exc = exc.InnerException;
                } while (exc != null);

                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet, FileDownload]
        public FileResult CancelamentoModelo1(int contratoId, string numContrato)
        {
            try
            {
                var pasta = Server.MapPath("~/TemplateRdlc/");

                if (numContrato != null)
                    numContrato = numContrato.TrimStart().Trim().TrimEnd();

                var via = _appsvcContr.GerarCartaCancelamentoModelo1(contratoId, pasta, numContrato);

                return File(via, "application/pdf", "cancelamento.pdf");
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                Exception exc = ex;
                do
                {
                    Response.StatusDescription += exc.Message + @"\r\n";
                    exc = exc.InnerException;
                } while (exc != null);

                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult EnviarEmailModeloCancelamentoUm(int contratoId, string numContrato)
        {
            try
            {
                var pasta = Server.MapPath("~/TemplateRdlc/");

                if (numContrato != null)
                    numContrato = numContrato.TrimStart().Trim().TrimEnd();

                _appsvcContr.EnviarCartaCancelamento1(contratoId, pasta, numContrato);

                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        [HttpGet, FileDownload]
        public FileResult CancelamentoModelo2(int contratoId, string numContrato)
        {
            try
            {
                var pasta = Server.MapPath("~/TemplateRdlc/");

                if (numContrato != null)
                    numContrato = numContrato.TrimStart().Trim().TrimEnd();

                var via = _appsvcContr.GerarCartaCancelamentoModelo2(contratoId, pasta, numContrato);

                return File(via, "application/pdf", "cancelamento.pdf");
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                Exception exc = ex;
                do
                {
                    Response.StatusDescription += exc.Message + @"\r\n";
                    exc = exc.InnerException;
                } while (exc != null);

                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult EnviarEmailModeloCancelamentoDois(int contratoId, string numContrato)
        {
            try
            {
                var pasta = Server.MapPath("~/TemplateRdlc/");

                if (numContrato != null)
                    numContrato = numContrato.TrimStart().Trim().TrimEnd();

                _appsvcContr.EnviarCartaCancelamento2(contratoId, pasta, numContrato);

                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        [HttpGet, FileDownload]
        public FileResult CancelamentoModelo3(int contratoId, string numContrato)
        {
            try
            {
                var pasta = Server.MapPath("~/TemplateRdlc/");

                if (numContrato != null)
                    numContrato = numContrato.TrimStart().Trim().TrimEnd();

                var via = _appsvcContr.GerarCartaCancelamentoModelo3(contratoId, pasta, numContrato);

                return File(via, "application/pdf", "cancelamento.pdf");
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                Exception exc = ex;
                do
                {
                    Response.StatusDescription += exc.Message + @"\r\n";
                    exc = exc.InnerException;
                } while (exc != null);

                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult EnviarEmailModeloCancelamentoTres(int contratoId, string numContrato)
        {
            try
            {
                var pasta = Server.MapPath("~/TemplateRdlc/");

                if (numContrato != null)
                    numContrato = numContrato.TrimStart().Trim().TrimEnd();

                _appsvcContr.EnviarCartaCancelamento3(contratoId, pasta, numContrato);

                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        [HttpGet, FileDownload]
        public FileResult CartaCobranca(int contratoId, string numContrato)
        {
            try
            {
                var pasta = Server.MapPath("~/TemplateRdlc/");
                var numeroContrato = 0;

                int.TryParse(numContrato, out numeroContrato);

                var via = _appsvcContr.GerarCartaCobranca(contratoId, pasta, numeroContrato);

                return File(via, "application/pdf", "cobrança.pdf");
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                Exception exc = ex;
                do
                {
                    Response.StatusDescription += exc.Message + @"\r\n";
                    exc = exc.InnerException;
                } while (exc != null);

                return null;
            }
        }

        public JsonResult BuscarClientes(string term)
        {
            if (string.IsNullOrEmpty(term))
                return Json(new { }, JsonRequestBehavior.AllowGet);

            int.TryParse(term, out var termid);
            var lst = _appsvcCli.Where(x => !x.Removido &&
                                            (
                                                x.RazaoSocial.ToLower().Contains(term.ToLower())
                                                || x.Fantasia.ToLower().Contains(term.ToLower())
                                                || (termid > 0 && x.ClienteId.Equals(termid))
                                            )).Select(x => new
            {
                id = x.ClienteId,
                text = x.RazaoSocial
            }).ToList();

            return Json(new
            {
                results = lst,
                pagination = new
                {
                    more = false
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarVendedores(string term)
        {
            if (string.IsNullOrEmpty(term))
                return Json(new {results = ""}, JsonRequestBehavior.AllowGet);

            int.TryParse(term, out var termid);
            var lst = _appsvcVnd.Where(x => !x.Removido &&
                                            (
                                                x.Nome.ToLower().Contains(term.ToLower())
                                                || x.NomeReduzido.ToLower().Contains(term.ToLower())
                                                || (termid > 0 && x.VendedorId.Equals(termid))
                                            )).Select(x => new
            {
                id = x.VendedorId,
                text = x.Nome
            }).ToList();

            return Json(new
            {
                results = lst,
                pagination = new
                {
                    more = false
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarTodosVendedores()
        {
            var lst = _appsvcVnd.Where(x => !x.Removido).Select(x => new
            {
                id = x.VendedorId,
                text = x.Nome
            }).ToList();

            return Json(new
            {
                results = lst,
                pagination = new
                {
                    more = false
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarProdutos(string term)

        {
            if (string.IsNullOrEmpty(term))
                return Json(new {results = ""}, JsonRequestBehavior.AllowGet);

            int.TryParse(term, out var termid);
            var lst = _appsvcPrd.Where(x => !x.Removido &&
                                            (
                                                x.Categoria.ToLower().Contains(term.ToLower())
                                                || x.Especie.ToLower().Contains(term.ToLower())
                                                || x.Descricao.ToLower().Contains(term.ToLower())
                                                || (termid > 0 && x.ProdutoId.Equals(termid))
                                            )).Select(x => new
            {
                id = x.ProdutoId,
                text = x.Descricao,
                valor = x.PrecoBase
            }).ToList();

            return Json(new
            {
                results = lst,
                pagination = new
                {
                    more = false
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarEmpresas(string term)

        {
            if (string.IsNullOrEmpty(term))
                return Json(new {results = ""}, JsonRequestBehavior.AllowGet);

            int.TryParse(term, out var termid);
            var lst = _appsvcEmp.Where(x => (
                x.RazaoSocial.ToLower().Contains(term.ToLower())
                || x.Fantasia.ToLower().Contains(term.ToLower())
                || x.Cnpj.ToLower().Contains(term.ToLower())
                || (termid > 0 && x.EmpresaId.Equals(termid))
            )).Select(x => new
            {
                id = x.EmpresaId,
                text = x.RazaoSocial
            }).ToList();

            return Json(new
            {
                results = lst,
                pagination = new
                {
                    more = false
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarCondicoes(int formaId)
        {
            var lst = _appsvcCond
                .Where(x => !x.Removido && x.FormasPagamentos.Select(y => y.FormaPagamentoId).Contains(formaId))
                .ToList().Select(x => new
                {
                    id = x.CondicaoPagamentoId,
                    text = x.Descricao
                }).ToList();


            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ContratoExiste(int numero, int contratoId = 0)
        {
            var ctr = _appsvcContr.Where(x => x.ContratoId != contratoId && x.Numero.Equals(numero)).FirstOrDefault();
            return Json(ctr == null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterDadosDashboard()
        {
            var ini = DateTime.Now.AddDays(-30).Date;
            var fim = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            var cea = _appsvcContr.Where(x => x.StatusContratoId == (int) EnumStatusContrato.EmAnalise).Count();
            var ca = _appsvcContr.Where(x => x.StatusContratoId == (int) EnumStatusContrato.Aprovado).Count();
            var cr = _appsvcContr.Where(x => x.StatusContratoId == (int) EnumStatusContrato.Reprovado).Count();

            var vlr = _appsvcContr.Where(x => x.StatusContratoId == (int) EnumStatusContrato.Aprovado
                                              && x.DataAprovacao.HasValue
                                              && x.DataAprovacao.Value >= ini
                                              && x.DataAprovacao.Value <= fim).ToList().Sum(x => x.ValorFinal);

            var stts = _appsvcStt.List().OrderBy(x => x.StatusContratoId).ToList();

            var contrs = _appsvcContr.Where(x => x.DataCriacao >= ini
                                                 && x.DataCriacao <= fim).ToList();

            var pordia = stts.Select(x =>
                contrs.Count(y => y.StatusContratoId == x.StatusContratoId)
            ).ToArray();

            return Json(new
            {
                EmAnalise = cea,
                Aprovados = ca,
                Reprovados = cr,
                Valor = vlr.ToString("N2"),
                LabelsPizza = stts.Select(x => x.Descricao).ToList(),
                PorStatus = pordia
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterMotivosSuspensao()
        {
            try
            {
                var lst = _appsvcMot.List();
                return Json(new { error = false, dados = lst}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteContract(int id)
        {
            try
            {
                var contract = _appsvcContr.Get(id);

                if (contract != null && contract.ContratoId > 0)
                {
                    var lista = contract.Titulos.ToList();

                    foreach(var tit in lista)
                    {
                        var boletos = tit.Boletos.ToList();

                        if(boletos.Any())
                        {
                            foreach (var boleto in boletos)
                            {
                                _appsvcTit.RemoverBoleto(boleto.BoletoId);
                            }
                        }

                        _appsvcTit.Delete(tit);
                    }

                    _appsvcContr.Delete(contract);
                }
                else
                    return Json(new { error = true, msg = "O contrato não existe" });

                return Json(new { error = false, msg = "" });
            }
            catch(Exception ex)
            {
                return Json(new { error = true, msg = ex.Message });
            }
        }

        private List<ContratoViewModel> ColocarPermissoesBotoes(List<ContratoViewModel> lista)
        {
            var dadosUsuario = (Usuario)Session["Usuario"];
            var permissoes = _appsvcPrmsUsr.Where(w => w.UsuarioId == dadosUsuario.UsuarioId).ToList();

            var dowload = permissoes.Where(d => d.Permissao != null && !d.Permissao.Removido && d.Permissao.Role == "DOWCTR").FirstOrDefault() == null ? false : true;
            var cancelamento = permissoes.Where(d => d.Permissao != null && !d.Permissao.Removido && d.Permissao.Role == "CDCAN").FirstOrDefault() == null ? false : true;
            var email = permissoes.Where(d => d.Permissao != null && !d.Permissao.Removido && d.Permissao.Role == "CONEM").FirstOrDefault() == null ? false : true;

            foreach(var item in lista)
            {
                item.PermiteDownload = dowload;
                item.PermiteCartaCancelamento = cancelamento;
                item.PermiteEnviarEmail = email;
            }

            return lista;
        }

        private int RetornarUsuarioParaFiltrar()
        {
            var usuario = 0;

            var dadosUsuario = (Usuario)Session["Usuario"];
            var permissoes = _appsvcPrmsUsr.Where(w => w.UsuarioId == dadosUsuario.UsuarioId).ToList();

            var resultado = permissoes.Where(p => p.Permissao != null && p.Permissao.Role == "VPCON").FirstOrDefault();

            if (resultado != null)
                usuario = dadosUsuario.UsuarioId;

            return usuario;
        }

        [HttpPost]
        [EdtespAuthorize(Roles = "VSCONTR")]
        public JsonResult VoltarSuspenso(int id)
        {
            var error = false;
            var msg = "";

            try
            {
                var contrato = _appsvcContr.Get(id);

                if (contrato == null)
                {
                    error = true;
                    msg = "O contrato não existe";
                }
                else
                {
                    if (contrato.StatusContratoId != (int)EnumStatusContrato.Suspenso)
                    {
                        error = true;
                        msg = "O contrato não está suspenso. Essa operação só pode ser realiza com contratos suspensos.";
                    }
                    else
                    {
                        contrato.StatusContratoId = (int)EnumStatusContrato.Aprovado;

                        var titulos = _appsvcTit.Where(x => x.ContratoId == contrato.ContratoId && x.Suspenso && !x.Removido && !x.DataBaixa.HasValue).ToList();

                        if(titulos != null && titulos.Any())
                        {
                            foreach(var item in titulos)
                            {
                                item.Suspenso = false;

                                if (item.Saldo == 0)
                                    item.Saldo = item.Valor;

                                _appsvcTit.AddOrUpdate(item);
                            }
                        }

                        _appsvcContr.AddOrUpdate(contrato);
                    }
                }
            }
            catch(Exception ex)
            {
                error = true;
                msg = ex.Message;
            }

            return Json(new { error = error, msg = msg });
        }


        [HttpPost]
        [EdtespAuthorize(Roles = "AVCONTR")]
        public JsonResult EditarDados(int id, int vend)
        {
            var error = false;
            var msg = "";
            
            try
            {
                var contrato = _appsvcContr.Get(id);

                if (contrato == null || vend <= 0)
                {
                    error = true;
                    msg = "O contrato não existe ou o selecione um vendedor";
                }
                else
                {
                    if (contrato.StatusContratoId != (int)EnumStatusContrato.Aprovado)
                    {
                        error = true;
                        msg = "O contrato não está aprovado. Essa operação só pode ser realiza com contratos aprovados.";
                    }
                    else
                    {
                        contrato.VendedorId = Convert.ToInt32(vend);

                        _appsvcContr.AddOrUpdate(contrato);
                    }
                }
            }
            catch (Exception ex)
            {
                error = true;
                msg = ex.Message;
            }

            return Json(new { error = error, msg = msg });
        }

        public JsonResult BuscarTodosFormasPagamento()
        {
            var lst = _appsvcForma.Where(x => !x.Removido).Select(x => new
            {
                id = x.FormaPagamentoId,
                text = x.Descricao
            }).ToList();

            return Json(new
            {
                results = lst,
                pagination = new
                {
                    more = false
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RetornarDadosParaEditarFormasDePagamento(int id)
        {
            var resultado = new EdicaoParcelasFormasPagamentoViewModel();

            var contrato = _appsvcContr.Get(id);

            if (contrato != null && contrato.ContratoId > 0)
            {
                resultado.Parcelas = this.RetornarParcelaResumida(id);

                if (resultado.Parcelas != null && resultado.Parcelas.Any())
                    resultado.FormasPagamento = this.RetornarFormasPagamentoResumido();
            }

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditarFormaPagmentoParcelaContrato(string dados)
        {
            var erro = false;
            var mensagem = "";

            try
            {
                if(!string.IsNullOrEmpty(dados))
                {
                    var parcelas = JsonConvert.DeserializeObject<List<ParcelaResumidoViewModel>>(dados);

                    if(parcelas != null && parcelas.Any())
                    {
                        foreach (var item in parcelas)
                        {
                            if (item.FormaPagamentoId > 0)
                            {
                                var parcela = _appsvcTit.Get(item.Id);
                                parcela.FormaPagamentoId = item.FormaPagamentoId;

                                _appsvcTit.AddOrUpdate(parcela);
                            }
                        }
                    }

                    mensagem = "Atualização feita com sucesso";
                }
                else
                {
                    erro = true;
                    mensagem = "Não há dados para atualizar";
                }
            }
            catch(Exception ex)
            {
                erro = true;
                mensagem = ex.Message;
            }

            return Json(new { error = erro, msg = mensagem });
        }

        private List<ParcelaResumidoViewModel> RetornarParcelaResumida(int contratoId)
        {
            var resultado = new List<ParcelaResumidoViewModel>();

            var titulos = _appsvcTit.RetornarTitulosPorContrato(contratoId);

            if (titulos != null && titulos.Any())
            {
                foreach (var item in titulos)
                {
                    resultado.Add(new ParcelaResumidoViewModel()
                    {
                        FormaPagamentoId = item.FormaPagamentoId,
                        Id = item.TituloId,
                        Valor = item.Valor,
                        Numero = item.Parcela
                    });
                }
            }

            return resultado;
        }

        private List<FormaPagamentoResumidoViewModel> RetornarFormasPagamentoResumido()
        {
            var resultado = new List<FormaPagamentoResumidoViewModel>();

            var formas = _appsvcForma.Where(x => !x.Removido).ToList();

            foreach (var forma in formas)
            {
                resultado.Add(new FormaPagamentoResumidoViewModel()
                {
                    Descricao = forma.Descricao,
                    Id = forma.FormaPagamentoId
                });
            }

            return resultado;
        }

        [HttpPost]
        public JsonResult EnviarEmailModeloCartaCobranca(int contratoId, string numContrato)
        {
            try
            {
                var pasta = Server.MapPath("~/TemplateRdlc/");

                var contrato = 0;

                int.TryParse(numContrato, out contrato);

                _appsvcContr.EnviarCartaCobranca(contratoId, pasta, contrato);

                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        [HttpGet, FileDownload]
        public FileResult CancelamentoModelo4(int contratoId, string numContrato)
        {
            try
            {
                var pasta = Server.MapPath("~/TemplateRdlc/");

                if (numContrato != null)
                    numContrato = numContrato.TrimStart().Trim().TrimEnd();

                var via = _appsvcContr.GerarCartaCancelamentoModelo4(contratoId, pasta, numContrato);

                return File(via, "application/pdf", "cancelamento.pdf");
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                Exception exc = ex;
                do
                {
                    Response.StatusDescription += exc.Message + @"\r\n";
                    exc = exc.InnerException;
                } while (exc != null);

                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult EnviarEmailModeloCancelamentoQuatro(int contratoId, string numContrato)
        {
            try
            {
                var pasta = Server.MapPath("~/TemplateRdlc/");

                if (numContrato != null)
                    numContrato = numContrato.TrimStart().Trim().TrimEnd();

                _appsvcContr.EnviarCartaCancelamento4(contratoId, pasta, numContrato);

                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = true, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}