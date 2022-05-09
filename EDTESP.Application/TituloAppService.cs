using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Repositories;
using EDTESP.Infrastructure.CC.BNet;
using EDTESP.Infrastructure.CC.GDrive;
using EDTESP.Infrastructure.CC.GNet;
using EDTESP.Infrastructure.CC.Util;
using RazorEngine;
using RazorEngine.Templating;

namespace EDTESP.Application
{
    public class TituloAppService : AppServiceBase<Titulo>, ITituloAppService
    {
        private readonly IRepositoryBase<Boleto> _repoBol;
        private readonly IRepositoryBase<Empresa> _repoEmp;
        private readonly IRepositoryBase<ParametroBanco> _repoPrmBnc;
        private readonly IRepositoryBase<Remessa> _repoRem;
        private readonly IRepositoryBase<Contrato> _repoContr;
        private readonly IRepositoryBase<Titulo> _repoTit;
        private readonly IRepositoryBase<ContratoEvento> _repoContrEvent;

        public TituloAppService(IUnitOfWork unitOfWork, 
            IRepositoryBase<Titulo> repobase,
            IRepositoryBase<Boleto> repoBol,
            IRepositoryBase<Empresa> repoEmp,
            IRepositoryBase<ParametroBanco> repoPrmBnc,
            IRepositoryBase<Remessa> repoRem,
            IRepositoryBase<Contrato> repoContr,
            IRepositoryBase<ContratoEvento> repoContrEvent) : base(unitOfWork, repobase)
        {
            _repoBol = repoBol;
            _repoEmp = repoEmp;
            _repoPrmBnc = repoPrmBnc;
            _repoRem = repoRem;
            _repoContr = repoContr;
            _repoTit = repobase;
            _repoContrEvent = repoContrEvent;
        }

        public void GravarParcela(Titulo tit, Contrato contr, bool persiste = true)
        {
            tit.ContratoId = contr.ContratoId;
            tit.Contrato = contr;
            tit.ClienteId = contr.ClienteId;
            //tit.FormaPagamentoId = formaId;
            //tit.CondicaoPagamentoId = condId;
            tit.DataCriacao = DateTime.Now;
            tit.UsuarioCriadorId = contr.UsuarioAtualizadorId ?? 0;
            tit.Saldo = tit.Valor;
            tit.VendedorId = contr.VendedorId;

            tit.Boletos = new List<Boleto>();
            if (contr.FormaPagamento.GeraBoleto)
            {
                var bol = GerarBoleto(tit, contr.UsuarioAtualizadorId??0);
                tit.Boletos.Add(bol);
            }

            RepoBase.Insert(tit);

            if(persiste) UnitOfWork.Save();
        }

        public void GerarBoleto(int tituloId, int usuarioId)
        {
            var tit = RepoBase.Find(tituloId);

            if(tit == null)
                throw new Exception("Título não existe");

            var bol = GerarBoleto(tit, usuarioId);
            _repoBol.Insert(bol);

            tit.DataAtualizacao = DateTime.Now;
            tit.UsuarioAtualizadorId = usuarioId;
            RepoBase.Update(tit);
            UnitOfWork.Save();
        }

        public void GerarBoletos(List<int> boletosId, int modalidadeId, int usuarioId, string folderpath)
        {
            var prm = _repoPrmBnc.Find(modalidadeId);

            if(prm == null)
                throw new Exception("Parâmetro de Banco não encontrado");

            var bolscnab = new List<Boleto>();
            foreach (var bid in boletosId)
            {
                var bol = _repoBol.Find(bid);

                if(bol == null)
                    throw new Exception("Boleto não existe");

                var tit = RepoBase.Find(bol.TituloId);

                if (bol.DataVenctoReal < DateTime.Now)
                    bol.DataVenctoReal = DateTime.Now.AddDays(1);
                
                var nossonum = prm.UltimoNossoNum+1;
                KeyValuePair<string, string> boleto;
                bolscnab.Add(bol);
                switch (prm.Tipo)
                {
                        
                    case ModalidadeGeracaoBoleto.Api:
                        boleto = GNetHelper.EmitirBoleto(prm.Agencia, 
                            prm.Conta, nossonum.ToString(), bol.Valor, tit.Contrato.Descricao,
                            bol.DataVenctoReal,
                            prm.Juros,
                            prm.Multa,
                            tit.Cliente.RazaoSocial, tit.Cliente.TipoPessoa, tit.Cliente.Documento, tit.Cliente.Telefone,
                            tit.Cliente.Email, prm.InfoBoleto
                        );
                        break;
                    case ModalidadeGeracaoBoleto.Cnab:
                        boleto = BNetHelper.GerarBoleto(bol.Empresa, prm, tit.Cliente, nossonum, bol.BoletoId.ToString(), "09", bol.DataVenctoReal, bol.Valor);
                        break;
                    default:
                        throw new Exception("Tipo de parametrização desconhecida");
                }

                if (string.IsNullOrEmpty(boleto.Key))
                    throw new Exception("Boleto não foi gerado");

                bol.ModalidadeId = prm.ParametroBancoId;
                bol.Gerado = false;
                bol.DataEmissao = DateTime.Now;
                bol.NossoNumero = nossonum.ToString();
                bol.NossoNumeroBanco = boleto.Key;
                bol.Multa = prm.Multa;
                bol.Juros = prm.Juros;
                bol.CodigoBarras = boleto.Value.ClearNumber();

                if (bol.CodigoBarras == null)
                    bol.CodigoBarras = "";

                bol.EnviadoAoBanco = true;
                bol.DataEnvioBanco = DateTime.Now;
                _repoBol.Update(bol);

                prm.UltimoNossoNum = nossonum;
                _repoPrmBnc.Update(prm);
            }

            foreach (var bol in bolscnab)
            {
                GerarPdfBoletoBNet(bol, folderpath);
            }

            GerarCnab(prm, bolscnab, usuarioId, folderpath);
            UnitOfWork.Save();
        }

        public void BaixarboletosNaoGerados(string folderpath)
        {
            var bols = _repoBol.Where(x =>!x.Gerado && x.EnviadoAoBanco && !x.Cancelado).ToList();

            foreach (var bol in bols)
            {
                switch (bol.ParametroBanco.Tipo)
                {
                    case ModalidadeGeracaoBoleto.Api:
                        BaixarBoletoGnet(bol, folderpath);
                        break;

                    case ModalidadeGeracaoBoleto.Cnab:
                        GerarPdfBoletoBNet(bol, folderpath);
                        break;
                    default:
                        break;
                }
            }
        }

        public void EnviarBoletosAoCliente(string folderpath)
        {
            var bols = _repoBol.Where(x =>
                x.Gerado && !x.EnviadoAoCliente && !x.Cancelado).ToList();

            var idcontrs = bols.Select(x => x.Titulo.ContratoId).Distinct().ToList();

            foreach (var idcontr in idcontrs)
            {
                EnviarBoletosContrato(idcontr, folderpath);
            }
        }

        public void ProcessarBoletosGnet()
        {
            var bols = _repoBol.Where(x => !x.Cancelado && !x.Titulo.Suspenso && !x.Titulo.Removido && !x.DataPgto.HasValue && x.ParametroBanco.Tipo == ModalidadeGeracaoBoleto.Api).ToList();

            foreach (var bol in bols)
            {
                GNetHelper.VerificarPgto(bol, bol.ParametroBanco.Agencia, bol.ParametroBanco.Conta);

                if (bol.Saldo == 0)
                    continue;

                var tit = bol.Titulo;
                tit.Saldo = bol.Saldo;
                tit.DataBaixa = bol.DataPgto;
                tit.DataAtualizacao = DateTime.Now;
                tit.UsuarioAtualizadorId = null;
                tit.TipoBaixaId = (int) EnumTipoBaixa.Automatica;

                RepoBase.Update(tit);
                _repoBol.Update(bol);
            }
            UnitOfWork.Save();
        }

        public void EnviarBoletosContrato(int contratoId, string folderpath)
        {
            var contr = _repoContr.Find(contratoId);

            if(contr == null)
                throw new Exception("Contrato informado não existe");

            var tits = RepoBase.Where(x => x.ContratoId == contratoId).ToList();

            if(!tits.Any())
                throw new Exception("Contrato informado não possui titulos a receber");

            var bols = tits.SelectMany(x => x.Boletos).Where(x => !x.Cancelado && x.Gerado).ToList();

            if(!bols.Any())
                throw new Exception("Contrato informado não possui boletos a gerar");

            var emp = contr.Empresa;
            var cli = contr.Cliente;
            var attchs = bols.Select(x => Path.Combine(folderpath, x.NomePdf)).ToArray();

            var sub = $"Boletos de Cobrança - (Contrato no. {contr.Numero})";
            var emailcli = string.IsNullOrEmpty(cli.EmailCobranca) ? cli.Email : cli.EmailCobranca;

            var tos = new Dictionary<string, string>
            {
                {emailcli,cli.RazaoSocial}
            };

            var tpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin/templates/envio_boleto_contrato.html");
            var tp = File.ReadAllText(tpath);
            var body = Engine.Razor.RunCompile(tp, "envio_boleto", null, new
            {
                Subject = sub,
                Numero = contr.Numero,
                Empresa = emp.RazaoSocial
            });

            EmailHelper.SendEmail(tos, sub, body, attchs, opcao: contr.EmpresaId);
            bols.ForEach(x =>
            {
                x.EnviadoAoCliente = true;
                _repoBol.Update(x);
            });
            
            UnitOfWork.Save();
        }

        public void EnviarPorEmail(string folderpath, int boletoId)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var bol = _repoBol.Find(boletoId);

            if (bol == null)
                throw new Exception("Boleto não existe");

            var path = Path.Combine(folderpath, bol.NomePdf);
            if (!File.Exists(path))
                throw new Exception("Boleto não existe!");

            var sub = $"[{bol.Empresa.RazaoSocial}] Boleto ref. Contrato {bol.Titulo.Contrato.Numero} parcela {bol.Titulo.Parcela}";
            var emailcli = string.IsNullOrEmpty(bol.Cliente.EmailCobranca) ? bol.Cliente.Email : bol.Cliente.EmailCobranca;

            var tos = new Dictionary<string, string>
            {
                {emailcli,bol.Cliente.RazaoSocial}
            };

            var tpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Templates", "envio_boleto.html");
            var tp = File.ReadAllText(tpath);
            var body = Engine.Razor.RunCompile(tp, "envio_boleto2", null, new
            {
                Subject = sub,
                Numero = bol.Titulo.Contrato.Numero,
                Empresa = bol.Empresa.RazaoSocial,
                Vencto = bol.DataVenctoReal.ToString("dd/MM/yyyy")
            });

            EmailHelper.SendEmail(tos, sub, body, new[] { path }, opcao: bol.EmpresaId);
        }

        public void AlterarVencto(int tituloId, DateTime novoVencto, int usuarioId, decimal? valorTotal)
        {
            var tit = RepoBase.Find(tituloId);

            if(tit == null)
                throw new Exception("Título não existe");

            //if(tit.Boletos.Any(x => !x.Cancelado && x.Gerado))
            //    throw new Exception("Vencimento não pode ser alterado, pois existem boletos emitidos.");

            if(novoVencto.Date < tit.DataCriacao.Date)
                throw new Exception("Vencimento não pode ser inferior a criação do mesmo");

            var bol = tit.Boletos.LastOrDefault(x => !x.Cancelado && !x.Gerado);

            if (bol != null)
            {
                bol.DataVenctoReal = novoVencto;

                if (valorTotal.HasValue && valorTotal.Value > 0 && bol.Valor != valorTotal.Value)
                {
                    bol.Valor = valorTotal.Value;

                    if (bol.Saldo > valorTotal.Value)
                        bol.Saldo = bol.Saldo - (bol.Saldo - valorTotal.Value);
                    else
                        bol.Saldo = bol.Saldo + (valorTotal.Value - tit.Saldo);
                }

                _repoBol.Update(bol);
            }

            tit.DataVenctoReal = novoVencto;
            tit.UsuarioAtualizadorId = usuarioId;
            tit.DataAtualizacao = DateTime.Now;

            if (valorTotal.HasValue && valorTotal.Value > 0 && valorTotal.Value != tit.Valor)
            {
                tit.Valor = valorTotal.Value;

                if (tit.Saldo > valorTotal.Value)
                    tit.Saldo = tit.Saldo - (tit.Saldo - valorTotal.Value);
                else
                    tit.Saldo = tit.Saldo + (valorTotal.Value- tit.Saldo);
            }

            RepoBase.Update(tit);
            UnitOfWork.Save();
        }

        public void CancelarBoleto(int boletoId, int usuarioId, bool novoBoleto = false)
        {
            var bol = _repoBol.Find(boletoId);

            if(bol == null)
                throw new Exception("Boleto não existe");

            if(bol.Cancelado)
                throw new Exception("Boleto já esta cancelado");

            var tit = bol.Titulo;
            tit.DataAtualizacao = DateTime.Now;
            tit.UsuarioAtualizadorId = usuarioId;

            CancelarBoleto(bol);

            if (novoBoleto)
            {
                var nbol = GerarBoleto(tit, usuarioId);
                _repoBol.Insert(nbol);
            }
            else
            {
                tit.DataCancelamento = DateTime.Now;
                tit.Suspenso = true;
            }

            RepoBase.Update(tit);
            UnitOfWork.Save();
        }

        public void BaixarTitulo(int tituloId, int usuarioId, DateTime dataBaixa, bool naoGerarExcecao = false)
        {
            var tit = RepoBase.Find(tituloId);

            if (tit == null)
            {
                if (!naoGerarExcecao)
                    throw new Exception("Boleto não existe");
                else
                    return;
            }

            if (tit.Suspenso)
            {
                if (!naoGerarExcecao)
                    throw new Exception("Titulo suspenso, não pode ser baixado");
                else
                    return;
            }

            tit.Saldo = 0;
            tit.DataBaixa = dataBaixa;
            tit.DataProcessamento = DateTime.Now;
            tit.DataAtualizacao = DateTime.Now;
            tit.UsuarioAtualizadorId = usuarioId;
            tit.TipoBaixaId = (int)EnumTipoBaixa.Manual;

            var bol = tit.Boletos.FirstOrDefault(x => !x.Cancelado && x.Gerado);

            if (bol != null)
            {
                bol.Saldo = 0;
                bol.DataPgto = dataBaixa;
                bol.TipoBaixaId = (int)EnumTipoBaixa.Manual;

                try
                {
                    if (bol.ParametroBanco.Tipo == ModalidadeGeracaoBoleto.Api)
                        GNetHelper.MarcarBoletoPago(bol.ParametroBanco.Agencia, bol.ParametroBanco.Conta, bol.NossoNumeroBanco);
                }
                catch (Exception)
                {
                    //ignored
                }

                _repoBol.Update(bol);
            }

            RepoBase.Update(tit);
            UnitOfWork.Save();

            var contrato = tit.Contrato;

            _repoContrEvent.Insert(new ContratoEvento()
            {
                ContratoId = contrato.ContratoId,
                DataCriacao = DateTime.Now,
                StatusContratoId = contrato.StatusContratoId,
                UsuarioCriadorId = usuarioId,
                Observacao = "Marcou como pago título " + tit.TituloId
            });
        }

        public void RemoverTitulos(IEnumerable<Titulo> tits, int usuarioId, bool persist = false)
        {
            var bols = tits.SelectMany(x => x.Boletos);

            foreach (var boleto in bols)
            {
                if(boleto.Cancelado)
                    continue;

                CancelarBoleto(boleto);
            }

            foreach (var tit in tits)
            {
                tit.Removido = true;
                tit.DataAtualizacao = DateTime.Now;
                tit.UsuarioAtualizadorId = usuarioId;

                RepoBase.Update(tit);
            }

            if(persist)
                UnitOfWork.Save();
        }

        private void CancelarBoleto(Boleto bol)
        {
            bol.Cancelado = true;

            try
            {
                if (bol.ParametroBanco.Tipo == ModalidadeGeracaoBoleto.Api)
                    GNetHelper.CancelarBoleto(bol.ParametroBanco.Agencia, bol.ParametroBanco.Conta, bol.NossoNumeroBanco);
            }
            catch (Exception)
            {
                //ignored
            }
            _repoBol.Update(bol);
        }

        private Boleto GerarBoleto(Titulo tit, int usuarioId)
        {
            var bol = new Boleto
            {
                TituloId = tit.TituloId,
                ClienteId = tit.ClienteId,
                EmpresaId = tit.Contrato.EmpresaId,
                ModalidadeId = null,
                DataEmissao = DateTime.Now,
                DataVencto = tit.DataVencto,
                DataVenctoReal = tit.DataVenctoReal,
                Valor = tit.Valor,
                Saldo = tit.Saldo,
                EnviadoAoBanco = false,
                Cancelado = false,
                UsuarioCriadorId = usuarioId
            };

            return bol;
        }

        private void BaixarBoletoGnet(Boleto bol, string folderpath)
        {
            var cid = Convert.ToInt32(bol.NossoNumeroBanco);
            var bx = GNetHelper.BaixarBoleto(bol.ParametroBanco.Agencia, bol.ParametroBanco.Conta, cid);

            //using (var stream = new MemoryStream(bx.Value))
            //{
            //    GDrive.UploadFile(stream, $"boleto {bx.Key}.pdf", "application/pdf", bx.Key.ToString());
            //}

            var filename = $"BOL_{bol.CodigoBarras}.pdf";
            var path = Path.Combine(folderpath, filename);

            File.WriteAllBytes(path, bx.Value);
            bol.Gerado = true;
            bol.NomePdf = filename;
            _repoBol.Update(bol);
            UnitOfWork.Save();
        }

        private void GerarPdfBoletoBNet(Boleto bol, string folderpath)
        {
            var cli = RepoBase.Find(bol.TituloId)?.Cliente;
            var filename = $"BOL_{bol.CodigoBarras}.pdf";

            var prm = _repoPrmBnc.Find(bol.ModalidadeId.Value);

            BNetHelper.GerarBoleto(cli, prm, bol, filename, folderpath);
            bol.Gerado = true;
            bol.NomePdf = filename;
            _repoBol.Update(bol);
            UnitOfWork.Save();
        }
        
        private void GerarCnab(ParametroBanco prm, List<Boleto> bols, int usuarioId, string folderpath)
        {
            var emps = bols.Select(x => x.EmpresaId).Distinct().ToList();
            var error = false;
            var errmsg = new StringBuilder();

            foreach (var idemp in emps)
            {
                try
                {
                    var empbols = bols.Where(x => x.EmpresaId == idemp).ToList();

                    var emp = empbols.Select(x => x.Empresa).First();

                    if(!empbols.Any())
                        continue;

                    var seq = prm.UltimoCnab + 1;
                    //var fname = $"{DateTime.Now:yyyy-MM-dd HH_mm_ss}_{seq.ToString().PadLeft(6,'0')}.txt";
                    var fname = $"{seq.ToString().PadLeft(6, '0')}.txt";

                    switch (prm.Tipo)
                    {
                        case ModalidadeGeracaoBoleto.Cnab:
                            BNetHelper.GerarRemessa(emp, prm, empbols, folderpath, fname, seq);
                            break;
                        case ModalidadeGeracaoBoleto.Api:
                            GNetHelper.GerarRemessa(emp, bols, folderpath, fname, seq);
                            break;
                    }

                    var rem = new Remessa
                    {
                        Baixada = false,
                        DataCriacao = DateTime.Now,
                        NomeTxt = fname,
                        UsuarioCriadorId = usuarioId,
                        ParametroBancoId = prm.ParametroBancoId
                    };
                    _repoRem.Insert(rem);
                    UnitOfWork.Save();

                    empbols.ForEach(x => x.RemessaId = rem.RemessaId);
                    
                    prm.UltimoCnab = seq;
                    _repoPrmBnc.Update(prm);
                    UnitOfWork.Save();
                }
                catch (Exception e)
                {
                    error = true;
                    errmsg.AppendLine(e.Message);
                }
            }

            if(error)
                throw new Exception(errmsg.ToString());
        }

        public bool RemoverBoleto(int boletoId)
        {
            var boleto = _repoBol.Find(boletoId);

            if (boleto != null && boleto.BoletoId > 0)
            {
                var titulo = boleto.Titulo;


                _repoBol.Delete(boleto);

                UnitOfWork.Save();

                return true;

            }

            return false;
        }

        public string ProcessarArquivoRetorno(Stream stream, int usuarioId)
        {
            var resultado = "";

            var listaResultado = BNetHelper.LerArquivoRetornoPeloStream(stream);

            if (listaResultado != null && listaResultado.Any())
            {
                foreach(var item in listaResultado)
                {
                    var boleto = _repoBol.Find(Convert.ToInt32(item.NumeroDocumento));

                    if(boleto != null)
                    {
                        var titulo = Get(boleto.TituloId);

                        if(titulo != null && titulo.TituloId > 0)
                        {
                            boleto.TipoBaixaId = 3;
                            boleto.Saldo = 0;
                            boleto.DataPgto = item.DataCredito;

                            _repoBol.Update(boleto);
                        }

                        decimal saldo = titulo.Saldo - boleto.Valor;

                        if (saldo == 0 || saldo < 0)
                        {
                            titulo.TipoBaixaId = 3;
                            titulo.Saldo = 0;
                            titulo.DataBaixa = item.DataCredito;
                        }
                        else
                            titulo.Saldo = saldo;

                        RepoBase.AddOrUpdate(titulo);

                        UnitOfWork.Save();
                    }
                }
            }
            else
                resultado = "Não foi possível processar nenhuma linha do arquivo";

            return resultado;
        }

        public Dictionary<bool, string> MarcarComoNaoBaixado(int tituloId, int usuarioId)
        {
            var result = new Dictionary<bool, string>();

            try
            {
                var tit = RepoBase.Find(tituloId);

                if (tit == null)
                    result.Add(false, "Boleto não existe");
                else if (tit.Suspenso)
                    result.Add(false, "Titulo suspenso, não pode ser baixado");
                else
                {
                    tit.DataBaixa = null;
                    tit.DataProcessamento = null;
                    tit.DataAtualizacao = DateTime.Now;
                    tit.UsuarioAtualizadorId = usuarioId;
                    tit.TipoBaixaId = null;

                    var bol = tit.Boletos.Where(x => !x.Cancelado && x.Gerado && x.DataPgto == null);

                    if (bol != null)
                        tit.Saldo = bol.Sum(x => x.Saldo);

                    if (tit.Saldo <= 0)
                        tit.Saldo = 1;

                    RepoBase.Update(tit);
                    UnitOfWork.Save();

                    result.Add(true, "");
                }
            }
            catch(Exception ex)
            {
                result.Add(false, ex.Message);
            }

            return result;
        }

        public List<Titulo> RetornarTitulosPorContrato(int contratoId)
        {
            var resultado = RepoBase.Where(r => !r.Removido && r.ContratoId == contratoId && r.Saldo > 0).ToList();

            return resultado;
        }

        public void CancelarTitulo(int tituloId, int usuarioId)
        {
            var titulo = _repoTit.Find(tituloId);

            if (titulo == null)
                throw new Exception("Título não existe");

            if (titulo.Removido)
                throw new Exception("Titulo está removido");

            if(titulo.Suspenso)
                throw new Exception("Titulo está suspenso");


            titulo.DataAtualizacao = DateTime.Now;
            titulo.UsuarioAtualizadorId = usuarioId;
            titulo.DataCancelamento = DateTime.Now;
            titulo.Suspenso = true;

            RepoBase.Update(titulo);
            UnitOfWork.Save();
        }
    }
}