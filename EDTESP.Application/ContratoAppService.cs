using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Repositories;
using EDTESP.Infrastructure.CC.ReportViewer;
using EDTESP.Infrastructure.CC.Util;
using RazorEngine;
using RazorEngine.Templating;

namespace EDTESP.Application
{
    public class ContratoAppService : AppServiceBase<Contrato>, IContratoAppService
    {
        private readonly IRepositoryBase<StatusContrato> _repoStatus;
        private readonly IRepositoryBase<ContratoEvento> _repoContrEvt;
        private readonly IRepositoryBase<Titulo> _repoTit;
        private readonly ICondicaoPagamentoAppService _appsvcCond;
        private readonly ITituloAppService _appsvcTit;
        private readonly IRepositoryBase<MotivoSuspensao> _repoMot;
        private readonly IRepositoryBase<Produto> _repoProd;


        public ContratoAppService(IUnitOfWork unitOfWork,
            IRepositoryBase<Contrato> repoBase, 
            IRepositoryBase<StatusContrato> repoStatus,
            IRepositoryBase<ContratoEvento> repoContrEvt,
            ITituloAppService appsvcTit,
            ICondicaoPagamentoAppService appsvcCond,
            IRepositoryBase<MotivoSuspensao> repoMot,
            IRepositoryBase<Produto> repoProd
            ) : base(unitOfWork, repoBase)
        {
            _repoStatus = repoStatus;
            _repoContrEvt = repoContrEvt;
            _appsvcTit = appsvcTit;
            _appsvcCond = appsvcCond;
            _repoMot = repoMot;
            _repoProd = repoProd;
        }
        
        public new void Insert(Contrato obj)
        {
            obj.ValorFinal = obj.ValorBase + obj.Tarifas - obj.Desconto;
            obj.StatusContratoId = (int)EnumStatusContrato.EmAnalise;
            obj.DataUltimaSituacao = DateTime.Now;

            //var prds = _repoProd.Where(x => produtosId.Contains(x.ProdutoId)).ToList();
            //prds.ForEach(x => _repoProd.Attach(x));
            //obj.Produtos = prds;
            obj.ContratoEventos = new List<ContratoEvento>
            {
                new ContratoEvento
                {
                    ContratoId = obj.ContratoId,
                    UsuarioCriadorId = obj.UsuarioCriadorId,
                    DataCriacao = obj.DataCriacao,
                    StatusContratoId = obj.StatusContratoId,
                    Observacao = "Contrato criado"
                }
            };

            RepoBase.Insert(obj);
            UnitOfWork.Save();
        }

        public new void Update(Contrato obj)
        {
            obj.ContratoEventos.Add(new ContratoEvento
            {
                ContratoId = obj.ContratoId,
                UsuarioCriadorId = obj.UsuarioAtualizadorId.Value,
                DataCriacao = obj.DataCriacao,
                StatusContratoId = obj.StatusContratoId,
                Observacao = "Contrato alterado"
            });
            
            RepoBase.Update(obj);
            UnitOfWork.Save();
        }

        public void Aprovar(Contrato obj)
        {
        
            var nobj = RepoBase.Find(obj.ContratoId);

            if(nobj == null)
                throw new Exception("Contrato não encontrado");

            if(nobj.StatusContratoId != (int)EnumStatusContrato.EmAnalise)
                throw new Exception("Contrato não esta em situação de ser aprovado");

            nobj.UsuarioAtualizadorId = obj.UsuarioAtualizadorId;
            nobj.DataAtualizacao = DateTime.Now;
            nobj.DataUltimaSituacao = DateTime.Now;
            nobj.DataAprovacao = DateTime.Now;
            nobj.StatusContratoId = (int) EnumStatusContrato.Aprovado;

            var parcs = _appsvcCond.GerarParcelas(obj.CondicaoPagamentoId, obj.FormaPagamentoId, obj.CondicaoPagamento1ParcId, obj.FormaPagamento1ParcId, obj.DataVencto1Parc, obj.ValorFinal, obj.PrimeiraParcela).ToList();

            parcs.ForEach(x =>
            {
                _appsvcTit.GravarParcela(x, nobj, false);
            });

            nobj.ContratoEventos.Add(new ContratoEvento
            {
                ContratoId = nobj.ContratoId,
                UsuarioCriadorId = nobj.UsuarioCriadorId,
                DataCriacao = nobj.DataCriacao,
                StatusContratoId = nobj.StatusContratoId,
                Observacao = "Contrato Aprovado"
            });

            RepoBase.Update(nobj);
            UnitOfWork.Save();
        }

        public void Reprovar(Contrato obj)
        {
            var nobj = RepoBase.Find(obj.ContratoId);

            if (nobj == null)
                throw new Exception("Contrato não encontrado");

            if (nobj.StatusContratoId != (int)EnumStatusContrato.EmAnalise)
                throw new Exception("Contrato não esta em situação de ser reprovado");

            nobj.UsuarioAtualizadorId = obj.UsuarioAtualizadorId;
            nobj.DataAtualizacao = DateTime.Now;
            nobj.DataUltimaSituacao = DateTime.Now;
            nobj.DataAprovacao = DateTime.Now;
            nobj.StatusContratoId = (int)EnumStatusContrato.Reprovado;
            
            nobj.ContratoEventos.Add(new ContratoEvento
            {
                ContratoId = nobj.ContratoId,
                UsuarioCriadorId = nobj.UsuarioCriadorId,
                DataCriacao = nobj.DataCriacao,
                StatusContratoId = nobj.StatusContratoId,
                Observacao = "Contrato Reprovado"
            });

            RepoBase.Update(nobj);
            UnitOfWork.Save();
        }

        public void Suspender(Contrato obj, int motivoSuspensaoId, string observacao, int usuario)
        {
            var nobj = RepoBase.Find(obj.ContratoId);

            if (nobj == null)
                throw new Exception("Contrato não encontrado");

            if (nobj.StatusContratoId != (int)EnumStatusContrato.Aprovado)
                throw new Exception("Contrato não esta em situação de ser suspenso");

            var mot = _repoMot.Find(motivoSuspensaoId);

            if (mot == null)
                throw new Exception("Motivo da suspensão não encontrado");

            nobj.UsuarioAtualizadorId = usuario;
            nobj.DataAtualizacao = DateTime.Now;
            nobj.DataUltimaSituacao = DateTime.Now;
            nobj.StatusContratoId = (int)EnumStatusContrato.Suspenso;

            nobj.ContratoEventos.Add(new ContratoEvento
            {
                ContratoId = nobj.ContratoId,
                UsuarioCriadorId = usuario,
                DataCriacao = nobj.DataCriacao,
                StatusContratoId = nobj.StatusContratoId,
                Observacao = $"Contrato suspenso ({mot.Descricao}) {observacao}"
            });
            
            var tits = _appsvcTit.Where(x => x.ContratoId == nobj.ContratoId && x.Saldo > 0).ToList();

            tits.ForEach(x =>
            {
                x.Suspenso = true;
                _appsvcTit.Update(x);
            });

            RepoBase.Update(nobj);
            UnitOfWork.Save();
        }

        public void Reaprovar(Contrato obj)
        {
            var nobj = RepoBase.Find(obj.ContratoId);

            if (nobj == null)
                throw new Exception("Contrato não encontrado");

            if (nobj.StatusContratoId != (int)EnumStatusContrato.Suspenso)
                throw new Exception("Contrato não esta em situação de ser reaprovado");

            nobj.UsuarioAtualizadorId = obj.UsuarioAtualizadorId;
            nobj.DataAtualizacao = DateTime.Now;
            nobj.DataUltimaSituacao = DateTime.Now;
            nobj.DataAprovacao = DateTime.Now;
            nobj.StatusContratoId = (int)EnumStatusContrato.Aprovado;

            var aparcs = _appsvcTit.Where(x => x.Suspenso && !x.Removido && x.ContratoId == obj.ContratoId).ToList();
            _appsvcTit.RemoverTitulos(aparcs, obj.UsuarioAtualizadorId.Value);

            var parcs = _appsvcCond.GerarParcelas(obj.CondicaoPagamentoId, obj.FormaPagamentoId, obj.CondicaoPagamento1ParcId, obj.FormaPagamento1ParcId, obj.DataVencto1Parc, obj.ValorFinal, obj.PrimeiraParcela).ToList();

            parcs.ForEach(x =>
            {
                _appsvcTit.GravarParcela(x, nobj, false);
            });

            nobj.ContratoEventos.Add(new ContratoEvento
            {
                ContratoId = nobj.ContratoId,
                UsuarioCriadorId = nobj.UsuarioCriadorId,
                DataCriacao = nobj.DataCriacao,
                StatusContratoId = nobj.StatusContratoId,
                Observacao = "Contrato Reaprovado"
            });

            RepoBase.Update(nobj);
            UnitOfWork.Save();
        }

        public void Cancelar(Contrato obj, string observacaoCanc)
        {
            var nobj = RepoBase.Find(obj.ContratoId);

            if (nobj == null)
                throw new Exception("Contrato não encontrado");

            if (nobj.StatusContratoId != (int)EnumStatusContrato.EmAnalise)
                throw new Exception("Contrato não esta em situação de ser cancelado");

            nobj.UsuarioAtualizadorId = obj.UsuarioAtualizadorId;
            nobj.DataAtualizacao = DateTime.Now;
            nobj.DataUltimaSituacao = DateTime.Now;
            nobj.StatusContratoId = (int)EnumStatusContrato.Cancelado;

            nobj.ContratoEventos.Add(new ContratoEvento
            {
                ContratoId = nobj.ContratoId,
                UsuarioCriadorId = nobj.UsuarioCriadorId,
                DataCriacao = nobj.DataCriacao,
                StatusContratoId = nobj.StatusContratoId,
                Observacao = $"Contrato cancelado. {observacaoCanc}"
            });

            var tits = _repoTit.Where(x => x.ContratoId == nobj.ContratoId && x.Saldo > 0).ToList();
            tits.ForEach(x =>
            {
                x.Boletos.ToList().ForEach(b => { b.Cancelado = true; });
                x.Removido = true;
                _repoTit.Update(x);
            });

            RepoBase.Update(nobj);
            UnitOfWork.Save();
        }
        
        public byte[] GerarCartaCancelamentoModelo1(int contratoId, string pasta = "", string contrato = "")
        {
            var contr = RepoBase.Find(contratoId, x => x.Empresa);

            if (contr == null)
                throw new Exception("Contrato não encontrado");
            
            var pathLogo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/logotipos", contr.Empresa.LogoEmpresa);
            
            var dic = new Dictionary<string, IEnumerable>();

            var carta = new
            {
                Logotipo = $"file://{pathLogo}",
                Empresa = contr.Empresa.RazaoSocial,
                Fantasia = contr.Empresa.Fantasia,
                Apelido = contr.Empresa.Descricao,
                Data = $"São Paulo, {DateTime.Now.Day} de {DateTime.Now:MMMM} de {DateTime.Now.Year}",
                Cliente = contr.Cliente.RazaoSocial,
                Cnpj = contr.Cliente.Documento.FormataDocumento(contr.Cliente.TipoPessoa),
                Endereco = $"{contr.Cliente.Endereco}, {contr.Cliente.Numero} {contr.Cliente.Complemento}",
                BairroCidade = $"{contr.Cliente.Bairro}-{contr.Cliente.Cidade}/{contr.Cliente.Uf}",
                Cep = contr.Cliente.Cep.FormataCep(),
                Responsavel = contr.Cliente.Responsavel,
                Linha1 = $"{contr.Empresa.Fantasia} - {contr.Empresa.Endereco}, {contr.Empresa.Numero}-{contr.Empresa.Complemento}-Cep {contr.Empresa.Cep.FormataCep()}-{contr.Empresa.Bairro}-{contr.Empresa.Cidade}-{contr.Empresa.Uf}",
                Linha2 = $"Fones {contr.Empresa.Telefones}",
                Linha3 = $"Site: {contr.Empresa.Site} - E-mail: {contr.Empresa.Email}",
                Contrato = contrato
            };

            dic.Add("DataSet1", new []{carta}.ToList());
            var rdlcpath = "";

            if (string.IsNullOrEmpty(pasta))
                rdlcpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/rdlc", "carta_canc_geral.rdlc");
            else
                rdlcpath = pasta + "carta_canc_geral.rdlc";

            return ReportControl.GeneratePdf(rdlcpath, $"Cancelamento (Modelo 1)", dic);
        }

        public byte[] GerarCartaCancelamentoModelo2(int contratoId, string pasta = "", string contrato = "")
        {
            var contr = RepoBase.Find(contratoId, x => x.Empresa);

            if (contr == null)
                throw new Exception("Contrato não encontrado");

            var pathLogo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/logotipos", contr.Empresa.LogoEmpresa);

            var dic = new Dictionary<string, IEnumerable>();

            var carta = new
            {
                Logotipo = $"file://{pathLogo}",
                Empresa = contr.Empresa.RazaoSocial,
                Fantasia = contr.Empresa.Fantasia,
                Apelido = contr.Empresa.Descricao,
                Data = $"São Paulo, {DateTime.Now.Day} de {DateTime.Now:MMMM} de {DateTime.Now.Year}",
                Cliente = contr.Cliente.RazaoSocial,
                Cnpj = contr.Cliente.Documento.FormataDocumento(contr.Cliente.TipoPessoa),
                Endereco = $"{contr.Cliente.Endereco}, {contr.Cliente.Numero} {contr.Cliente.Complemento}",
                BairroCidade = $"{contr.Cliente.Bairro}-{contr.Cliente.Cidade}/{contr.Cliente.Uf}",
                Cep = contr.Cliente.Cep.FormataCep(),
                Responsavel = contr.Cliente.Responsavel,
                Linha1 = $"{contr.Empresa.Fantasia} - {contr.Empresa.Endereco}, {contr.Empresa.Numero}-{contr.Empresa.Complemento}-Cep {contr.Empresa.Cep.FormataCep()}-{contr.Empresa.Bairro}-{contr.Empresa.Cidade}-{contr.Empresa.Uf}",
                Linha2 = $"Fones {contr.Empresa.Telefones}",
                Linha3 = $"Site: {contr.Empresa.Site} - E-mail: {contr.Empresa.Email}",
                Contrato = contrato
            };

            dic.Add("DataSet1", new[] { carta }.ToList());
            var rdlcpath = "";

            if (string.IsNullOrEmpty(pasta))
                rdlcpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/rdlc", "carta_canc_total.rdlc");
            else
                rdlcpath = pasta + "carta_canc_total.rdlc";

            return ReportControl.GeneratePdf(rdlcpath, $"Cancelamento (Modelo 2)", dic);
        }

        public byte[] GerarCartaCancelamentoModelo3(int contratoId, string pasta = "", string contrato = "")
        {
            var contr = RepoBase.Find(contratoId, x => x.Empresa);

            if (contr == null)
                throw new Exception("Contrato não encontrado");

            var pathLogo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/logotipos", contr.Empresa.LogoEmpresa);

            var dic = new Dictionary<string, IEnumerable>();
            var tipop = contr.Cliente.TipoPessoa == TipoPessoa.Juridica ? "CNPJ" : "CPF";

            var tits = _appsvcTit.Where(x => x.ContratoId == contr.ContratoId && !x.Removido && !x.Suspenso && x.Saldo > 0).OrderBy(x => x.DataVenctoReal).ToList();

            var ft = tits.FirstOrDefault();
            var tot = tits.Sum(x => x.Saldo);

            var parcelas = this.RetornarParcelasContratoPorExtenso(tits);

            var carta = new
            {
                Logotipo = $"file://{pathLogo}",
                Data = $"São Paulo, {DateTime.Now.Day} de {DateTime.Now:MMMM} de {DateTime.Now.Year}",
                Contrato = contrato,

                Cliente = contr.Cliente.RazaoSocial,
                Endereco = $"{contr.Cliente.Endereco}, {contr.Cliente.Numero} {contr.Cliente.Complemento} - {contr.Cliente.Bairro} - Cep {contr.Cliente.Cep.FormataCep()}-{contr.Cliente.Cidade}/{contr.Cliente.Uf}",
                TipoDoc = $"{tipop}",
                Documento = contr.Cliente.Documento.FormataDocumento(contr.Cliente.TipoPessoa),
                Resp = contr.Cliente.Responsavel,

                Empresa = contr.Empresa.RazaoSocial,
                EnderecoEmp = $"{contr.Empresa.Endereco}, {contr.Empresa.Numero}-{contr.Empresa.Complemento}-Cep {contr.Empresa.Cep.FormataCep()}-{contr.Empresa.Bairro}-{contr.Empresa.Cidade}-{contr.Empresa.Uf}",
                Cnpj = contr.Empresa.Cnpj.FormataDocumento(TipoPessoa.Juridica),

                ValorTotal = tot,
                Parcelas = parcelas,
                ValorParcela = ft?.Valor,
                Vencto = ft?.DataVenctoReal,

                Linha1 = $"{contr.Empresa.Fantasia} - {contr.Empresa.Endereco}, {contr.Empresa.Numero}-{contr.Empresa.Complemento}-Cep {contr.Empresa.Cep.FormataCep()}-{contr.Empresa.Bairro}-{contr.Empresa.Cidade}-{contr.Empresa.Uf}",
                Linha2 = $"Fones {contr.Empresa.Telefones}",
                Linha3 = $"Site: {contr.Empresa.Site} - E-mail: {contr.Empresa.Email}",
            };

            dic.Add("DataSet1", new[] { carta }.ToList());
            var rdlcpath = "";

            if (string.IsNullOrEmpty(pasta))
                rdlcpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/rdlc", "carta_instr.rdlc");
            else
                rdlcpath = pasta + "carta_instr.rdlc";

            return ReportControl.GeneratePdf(rdlcpath, $"Cancelamento (Modelo 3)", dic);
        }

        public byte[] GerarCartaCobranca(int contratoId, string pasta = "", int numeroContrato = 0)
        {
            var contr = RepoBase.Find(contratoId, x => x.Empresa);

            if (contr == null)
                throw new Exception("Contrato não encontrado");

            var pathLogo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/logotipos", contr.Empresa.LogoEmpresa);

            var dic = new Dictionary<string, IEnumerable>();

            var carta = new
            {
                Logotipo = $"file://{pathLogo}",
                Empresa = contr.Empresa.RazaoSocial,
                Fantasia = contr.Empresa.Fantasia,
                Apelido = contr.Empresa.Descricao,
                Data = $"São Paulo, {DateTime.Now.Day} de {DateTime.Now:MMMM} de {DateTime.Now.Year}",
                Cliente = contr.Cliente.RazaoSocial,
                Cnpj = contr.Cliente.Documento.FormataDocumento(contr.Cliente.TipoPessoa),
                Endereco = $"{contr.Cliente.Endereco}, {contr.Cliente.Numero} {contr.Cliente.Complemento}",
                BairroCidade = $"{contr.Cliente.Bairro}-{contr.Cliente.Cidade}/{contr.Cliente.Uf}",
                Cep = contr.Cliente.Cep.FormataCep(),
                Responsavel = contr.Cliente.Responsavel,
                Linha1 = $"{contr.Empresa.Fantasia} - {contr.Empresa.Endereco}, {contr.Empresa.Numero}-{contr.Empresa.Complemento}-Cep {contr.Empresa.Cep.FormataCep()}-{contr.Empresa.Bairro}-{contr.Empresa.Cidade}-{contr.Empresa.Uf}",
                Linha2 = $"Fones {contr.Empresa.Telefones}",
                Linha3 = $"Site: {contr.Empresa.Site} - E-mail: {contr.Empresa.Email}",
                Contrato = numeroContrato <= 0 ? contr.Numero : numeroContrato
            };

            dic.Add("DataSet1", new[] { carta }.ToList());
            var rdlcpath = "";
            if (string.IsNullOrEmpty(pasta))
                rdlcpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/rdlc", "carta_cobr.rdlc");
            else
                rdlcpath = pasta + "carta_cobr.rdlc";

            return ReportControl.GeneratePdf(rdlcpath, $"Cobrança (Modelo 1)", dic);
        }

        public byte[] GerarViaContrato(int contratoId, string pasta = "")
        {
            var contr = RepoBase.Find(contratoId, x=> x.Empresa);

            if (contr == null)
                throw new Exception("Contrato não encontrado");

            return GerarContrato(contr, pasta);
        }

        public void EnviarContratoEmail(int contratoId, string pasta = "")
        {
            var contr = RepoBase.Find(contratoId);

            if(contr == null)
                throw new Exception("Contato não encontrado");

            var bff = GerarContrato(contr, pasta);

            var cli = contr.Cliente;
            var emp = contr.Empresa;
            var sub = $"Contrato de Publicidade - (Contrato no. {contr.Numero})";

            var emailcli = string.IsNullOrEmpty(cli.EmailCobranca) ? cli.Email : cli.EmailCobranca;

            var tos = new Dictionary<string, string>
            {
                {emailcli,cli.RazaoSocial}
            };

            var tpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/templates/envio_contrato.html");
            var tp = File.ReadAllText(tpath);
            var body = Engine.Razor.RunCompile(tp, "envio_contrato", null, new
            {
                Subject = sub,
                Numero = contr.Numero,
                Empresa = emp.RazaoSocial
            });

            var attachs = new Dictionary<string, byte[]>
            {
                {$"Contrato {contr.Numero}.pdf", bff }
            };

            EmailHelper.SendGridEmail(tos, sub, body, attachs);

            contr.EnviadoAoCliente = true;
            RepoBase.Update(contr);
            UnitOfWork.Save();
        }

        private byte[] GerarContrato(Contrato contr, string pasta = "")
        {
            if (contr.StatusContratoId == (int)EnumStatusContrato.Cancelado)
                throw new Exception("Contrato Cancelado, não pode ser impresso");

            var pathLogo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/logotipos", contr.Empresa.LogoEmpresa);

            var endcob = string.IsNullOrEmpty(contr.Cliente.CepCobr)
                ? $"{contr.Cliente.Endereco}, {contr.Cliente.Numero} {contr.Cliente.Complemento} {contr.Cliente.Bairro} {contr.Cliente.Cidade} {contr.Cliente.Uf}"
                : $"{contr.Cliente.EnderecoCobr}, {contr.Cliente.NumeroCobr} {contr.Cliente.ComplementoCobr} {contr.Cliente.BairroCobr} {contr.Cliente.CidadeCobr} {contr.Cliente.UfCobr}";

            var parcs = _appsvcTit.Where(x => !x.Removido && !x.Suspenso && x.ContratoId == contr.ContratoId).ToList();

            var valorNaoBoleto = parcs.Where(w => w.FormaPagamentoId != 6).Sum(s => s.Valor);

            var objds1 = new
            {
                Logotipo = $"file://{pathLogo}",
                ContratoId = contr.Numero,
                contr.AnoEdicao,
                AnoVeiculacao = contr.AnoEdicao,
                Representante = contr.Vendedor.NomeReduzido,
                Cargo = contr.Vendedor.Cargo.Descricao,
                CodigoRepresentante = contr.VendedorId,
                DataAssinatura = contr.DataInicio,
                contr.Cliente.RazaoSocial,
                contr.Cliente.Endereco,
                contr.Cliente.Numero,
                contr.Cliente.Complemento,
                contr.Cliente.Bairro,
                contr.Cliente.Cidade,
                UF = contr.Cliente.Uf,
                Cep = contr.Cliente.Cep.FormataCep(),
                Telefones = $"{contr.Cliente.Telefone.FormataTel()} {contr.Cliente.Celular.FormataCel()}",
                Cnpj = contr.Cliente.Documento.FormataDocumento(contr.Cliente.TipoPessoa),
                EnderecoCobranca = endcob,
                NomeCheque = contr.Empresa.RazaoSocial,
                Autorizante = contr.Cliente.Responsavel,
                Observacoes = contr.Observacao,
                contr.Empresa.Site,
                contr.Empresa.Email,
                DadosContratada = $"{contr.Empresa.RazaoSocial} {contr.Empresa.Endereco}, {contr.Empresa.Numero} {contr.Empresa.Complemento} {contr.Empresa.Bairro} {contr.Empresa.Cidade} CEP {contr.Empresa.Cep} Fones {contr.Empresa.Telefones} CNPJ {contr.Empresa.Cnpj.FormataDocumento("J")}",
                ValorRecibo = DecimalUtil.GetFullNameValue(valorNaoBoleto)
            };

            var lstprod = new[]
            {
                new
                {
                    Unidade = "01",
                    contr.Especie,
                    contr.Categoria,
                    contr.Descricao,
                    Valor = contr.ValorFinal
                }
            };

            //var parcs = contr.Titulos.Where(x => !x.Removido).ToList();
            var tp = parcs.Count;

            var dic = new Dictionary<string, IEnumerable>
            {
                {"DadosContrato", new[] {objds1}.ToList()},
                {"Produtos", lstprod}
            };

            if (parcs.Count >= 2)
            {
                var fp = parcs.OrderBy(x => x.DataVenctoReal).First();
                fp = _appsvcTit.Get(fp.TituloId, i => i.FormaPagamento);
                var sp = parcs.OrderBy(x => x.DataVenctoReal).ElementAt(1);
                var parc = new
                {
                    Parcela = $"1/{parcs.Count}",
                    Vencimento = fp.DataVenctoReal,
                    Descricao =
                        $"{fp.FormaPagamento?.Descricao}\r\nDemais parcelas ({parcs.Count - 1}) nos meses subsequentes no valor de {sp.Valor:C2}",
                    fp.Valor
                };

                dic.Add("Parcelas", new []{parc}.ToList());
            }
            else if(parcs.Any())
            {
                var fp = parcs.OrderBy(x => x.DataVenctoReal).First();
                var parc = new
                {
                    Parcela = $"1/{parcs.Count}",
                    Vencimento = fp.DataVenctoReal,
                    fp.FormaPagamento?.Descricao,
                    fp.Valor
                };
                dic.Add("Parcelas", new[] { parc }.ToList());
            }
            else
            {
                var parc = new
                {
                    Parcela = "",
                    Vencimento = DateTime.MinValue,
                    Descricao = "",
                    Valor = 0
                };
                dic.Add("Parcelas", new[] { parc }.ToList());
            }

            var rdlcpath = "";

            if (string.IsNullOrEmpty(pasta))
                rdlcpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/rdlc", "modelo1.rdlc");
            else
                rdlcpath = pasta + "modelo1.rdlc";

            return ReportControl.GeneratePdf(rdlcpath, $"Contrato {contr.ContratoId}", dic);
        }

        public override void Delete(Contrato obj)
        {
            var listEventsContract = obj.ContratoEventos.ToList();

            if (listEventsContract != null && listEventsContract.Any())
            {
                foreach (var item in listEventsContract)
                    _repoContrEvt.Delete(item);
            }

            RepoBase.Delete(obj);

            UnitOfWork.Save();
        }

        public void EnviarCartaCancelamento1(int contratoId, string pasta = "", string contrato = "")
        {
            var contr = RepoBase.Find(contratoId);

            if (contr == null)
                throw new Exception("Contato não encontrado");

            var bff = GerarCartaCancelamentoModelo1(contr.ContratoId, pasta, contrato);

            var cli = contr.Cliente;
            var emp = contr.Empresa;
            var sub = $"Carta de Cancelamento Geral - (Contrato no. {(contrato)})";

            var emailcli = string.IsNullOrEmpty(cli.EmailCobranca) ? cli.Email : cli.EmailCobranca;

            var tos = new Dictionary<string, string>
            {
                {emailcli,cli.RazaoSocial}
            };

            var attachs = new Dictionary<string, byte[]>
            {
                {$"CartaCancelamentoGeral.pdf", bff }
            };

            EmailHelper.SendGridEmail(tos, sub, "", attachs);
        }

        public void EnviarCartaCancelamento2(int contratoId, string pasta = "", string contrato = "")
        {
            var contr = RepoBase.Find(contratoId);

            if (contr == null)
                throw new Exception("Contato não encontrado");

            var bff = GerarCartaCancelamentoModelo2(contr.ContratoId, pasta, contrato);

            var cli = contr.Cliente;
            var emp = contr.Empresa;
            var sub = $"Carta de Cancelamento Total - (Contrato no. {contrato})";

            var emailcli = string.IsNullOrEmpty(cli.EmailCobranca) ? cli.Email : cli.EmailCobranca;

            var tos = new Dictionary<string, string>
            {
                {emailcli,cli.RazaoSocial}
            };

            var attachs = new Dictionary<string, byte[]>
            {
                {$"CartaCancelamentoTotal.pdf", bff }
            };

            EmailHelper.SendGridEmail(tos, sub, "", attachs);
        }

        public void EnviarCartaCancelamento3(int contratoId, string pasta = "", string contrato = "")
        {
            var contr = RepoBase.Find(contratoId);

            if (contr == null)
                throw new Exception("Contato não encontrado");

            var bff = GerarCartaCancelamentoModelo3(contr.ContratoId, pasta, contrato);

            var cli = contr.Cliente;
            var emp = contr.Empresa;
            var sub = $"Rescisão - (Contrato no. {contrato})";

            var emailcli = string.IsNullOrEmpty(cli.EmailCobranca) ? cli.Email : cli.EmailCobranca;

            var tos = new Dictionary<string, string>
            {
                {emailcli,cli.RazaoSocial}
            };

            var attachs = new Dictionary<string, byte[]>
            {
                {$"Rescisao.pdf", bff }
            };

            EmailHelper.SendGridEmail(tos, sub, "", attachs);
        }

        public void EnviarCartaCobranca(int contratoId, string pasta = "", int numeroContrato = 0)
        {
            var contr = RepoBase.Find(contratoId);

            if (contr == null)
                throw new Exception("Contato não encontrado");

            var bff = GerarCartaCobranca(contr.ContratoId, pasta, numeroContrato);

            var cli = contr.Cliente;
            var emp = contr.Empresa;
            var sub = $"Cobrança - (Contrato no. {(numeroContrato <= 0 ? contr.Numero : numeroContrato)})";

            var emailcli = string.IsNullOrEmpty(cli.EmailCobranca) ? cli.Email : cli.EmailCobranca;

            var tos = new Dictionary<string, string>
            {
                {emailcli,cli.RazaoSocial}
            };

            var attachs = new Dictionary<string, byte[]>
            {
                {$"cobranca.pdf", bff }
            };

            EmailHelper.SendGridEmail(tos, sub, "", attachs);
        }

        public byte[] GerarCartaCancelamentoModelo4(int contratoId, string pasta = "", string contrato = "")
        {
            var contr = RepoBase.Find(contratoId, x => x.Empresa);

            if (contr == null)
                throw new Exception("Contrato não encontrado");

            var pathLogo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/logotipos", contr.Empresa.LogoEmpresa);

            var tits = _appsvcTit.Where(x => x.ContratoId == contr.ContratoId && !x.Removido && !x.Suspenso && x.Saldo > 0).OrderBy(x => x.DataVenctoReal).ToList();

            var tit = tits.FirstOrDefault();

            decimal total = tits.Sum(s => s.Valor);
            var extensoTotal = DecimalUtil.GetFullNameValue(total);

            var data = tit != null ? tit.DataVenctoReal.ToString("dd/MM/yyyy") : "";

            var parcelasExtenso = this.RetornarParcelasContratoPorExtenso(tits);

            var dic = new Dictionary<string, IEnumerable>();

            var carta = new
            {
                Logotipo = $"file://{pathLogo}",
                Empresa = contr.Empresa.RazaoSocial,
                Fantasia = contr.Empresa.Fantasia,
                Apelido = contr.Empresa.Descricao,
                Data = $"São Paulo, {DateTime.Now.Day} de {DateTime.Now:MMMM} de {DateTime.Now.Year}",
                Cliente = contr.Cliente.RazaoSocial,
                Cnpj = contr.Cliente.Documento.FormataDocumento(contr.Cliente.TipoPessoa),
                Endereco = $"{contr.Cliente.Endereco}, {contr.Cliente.Numero} {contr.Cliente.Complemento}",
                BairroCidade = $"{contr.Cliente.Bairro}-{contr.Cliente.Cidade}/{contr.Cliente.Uf}",
                Cep = contr.Cliente.Cep.FormataCep(),
                Responsavel = contr.Cliente.Responsavel,
                Linha1 = $"{contr.Empresa.Fantasia} - {contr.Empresa.Endereco}, {contr.Empresa.Numero}-{contr.Empresa.Complemento}-Cep {contr.Empresa.Cep.FormataCep()}-{contr.Empresa.Bairro}-{contr.Empresa.Cidade}-{contr.Empresa.Uf}",
                Linha2 = $"Fones {contr.Empresa.Telefones}",
                Linha3 = $"Site: {contr.Empresa.Site} - E-mail: {contr.Empresa.Email}",
                Contrato = contrato,
                ValorTotal = total.ToString("c"),
                ValorTotalExtenso = " (" + extensoTotal + ")",
                DataUm = data,
                ParteParcelas = parcelasExtenso
            };

            dic.Add("DataSet1", new[] { carta }.ToList());
            var rdlcpath = "";

            if (!string.IsNullOrEmpty(pasta))
                rdlcpath = pasta + "carta_canc_geral_vinc.rdlc";

            return ReportControl.GeneratePdf(rdlcpath, $"Cancelamento (Modelo 4)", dic);
        }

        public void EnviarCartaCancelamento4(int contratoId, string pasta = "", string contrato = "")
        {
            var contr = RepoBase.Find(contratoId);

            if (contr == null)
                throw new Exception("Contato não encontrado");

            var bff = GerarCartaCancelamentoModelo4(contr.ContratoId, pasta, contrato);

            var cli = contr.Cliente;
            var emp = contr.Empresa;
            var sub = $"Carta de Cancelamento Geral Vinculada - (Contrato no. {(contrato)})";

            var emailcli = string.IsNullOrEmpty(cli.EmailCobranca) ? cli.Email : cli.EmailCobranca;

            var tos = new Dictionary<string, string>
            {
                {emailcli,cli.RazaoSocial}
            };

            var attachs = new Dictionary<string, byte[]>
            {
                {$"CartaCancelamentoGeralVinculada.pdf", bff }
            };

            EmailHelper.SendGridEmail(tos, sub, "", attachs);
        }

        private string RetornarParcelasContratoPorExtenso(List<Titulo> lista)
        {
            var resultado = "";

            var agrupamento = lista.GroupBy(g => new { g.FormaPagamentoId, g.Valor });

            if (agrupamento != null && agrupamento.Any())
            {
                var contador = 1;

                foreach (var item in agrupamento)
                {
                    var formaPagamento = item.ToList().First().FormaPagamento;

                    resultado += $"{item.ToList().Count} {formaPagamento.Descricao.ToLower()} no valor de {item.Key.Valor.ToString("c2")} ({DecimalUtil.GetFullNameValue(item.Key.Valor)})";

                    var verificador = (contador + 1);

                    if (verificador <= agrupamento.Count())
                        resultado += ", ";

                    contador++;
                }
            }

            return resultado;
        }
    }
}