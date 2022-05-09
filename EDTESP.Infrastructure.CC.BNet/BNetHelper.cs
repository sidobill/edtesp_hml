using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Boleto2Net;
using EDTESP.Domain.Entities;
using EDTESP.Infrastructure.CC.Util;
using Boleto = Boleto2Net.Boleto;

namespace EDTESP.Infrastructure.CC.BNet
{
    public class BNetHelper
    {
        private static Boleto CriaBoleto(Empresa empresa, ParametroBanco prm, Cliente cli, int nossoNum, string numDoc, string carteira, DateTime vencto, decimal valor)
        {
            var banco = Banco.Instancia(Convert.ToInt32(empresa.ParametroBanco.Banco));
            banco.Cedente = new Cedente
            {
                CPFCNPJ = prm.Cnpj,
                Nome = empresa.RazaoSocial,
                MostrarCNPJnoBoleto = true,
                Endereco = new Endereco
                {
                    CEP = empresa.Cep,
                    LogradouroEndereco = empresa.Endereco,
                    LogradouroNumero = empresa.Numero,
                    LogradouroComplemento = empresa.Complemento,
                    Bairro = empresa.Bairro,
                    Cidade = empresa.Cidade,
                    UF = empresa.Uf
                },
                ContaBancaria = new ContaBancaria
                {
                    Agencia = prm.Agencia,
                    DigitoAgencia = prm.AgenciaDv,
                    Conta = prm.Conta,
                    DigitoConta = prm.ContaDv,
                    CarteiraPadrao = prm.Carteira,
                    TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa,
                    TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                    TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro
                },
                Codigo = prm.Cedente,
                CodigoDV = prm.CedenteDv
            };


            var bol = new Boleto(banco)
            {
                DataVencimento = vencto,
                ValorTitulo = valor,
                NossoNumero = nossoNum.ToString(),
                Aceite = "N",
                DataEmissao = DateTime.Now,
                NumeroDocumento = numDoc,
                PercentualJurosDia = prm.Juros,
                PercentualMulta = prm.Multa,
                ValorJurosDia = Math.Round((valor * (prm.Juros / 100)), 2),
                ValorMulta = Math.Round((valor * (prm.Multa / 100)), 2),
                DataJuros = vencto,
                DataMulta = vencto,
                Sacado = new Sacado
                {
                    CPFCNPJ = cli.Documento,
                    Nome = cli.RazaoSocial,
                    Endereco = new Endereco
                    {
                        CEP = cli.Cep,
                        LogradouroEndereco = cli.Endereco,
                        LogradouroNumero = cli.Numero,
                        LogradouroComplemento = cli.Complemento,
                        Bairro = cli.Bairro,
                        Cidade = cli.Cidade,
                        UF = cli.Uf,
                    }
                }

            };
            banco.FormataNossoNumero(bol);
            bol.Banco.FormataCedente();
            banco.FormataCodigoBarraCampoLivre(bol);

            bol.ValidarDados();
            return bol;
        }

        private static Boleto2NetProxy GerarProxy(Empresa emp, ParametroBanco prm)
        {
            var proxy = new Boleto2NetProxy();
            string msg = null;

            proxy.SetupCobranca(prm.Cnpj, emp.RazaoSocial, emp.Endereco, emp.Numero,
                emp.Complemento, emp.Bairro, emp.Cidade, emp.Uf, emp.Cep, "",
                Convert.ToInt32(prm.Banco), prm.Agencia,
                prm.AgenciaDv, "", prm.Conta, prm.ContaDv, prm.Cedente,
                prm.CedenteDv, "", prm.Carteira, "", (int)TipoCarteira.CarteiraCobrancaSimples,
                (int)TipoFormaCadastramento.ComRegistro, (int)TipoImpressaoBoleto.Empresa,
                (int)TipoDocumento.Tradicional, ref msg);

            if(!string.IsNullOrEmpty(msg))
                throw new Exception(msg);

            return proxy;
        }

        private static void AdicionarBoleto(ref Boleto2NetProxy proxy, Cliente cli, Domain.Entities.Boleto obj)
        {
            string msg = null;

            proxy.NovoBoleto(ref msg);
            if (!string.IsNullOrEmpty(msg))
                throw new Exception(msg);

            proxy.DefinirSacado(cli.Documento, cli.RazaoSocial, cli.Endereco, cli.Numero, cli.Complemento, cli.Bairro,
                cli.Cidade, cli.Uf, cli.Cep, null, ref msg);

            if (!string.IsNullOrEmpty(msg))
                throw new Exception(msg);

            proxy.DefinirBoleto(null, obj.BoletoId.ToString(), obj.NossoNumero, obj.DataEmissao, DateTime.Now,
                obj.DataVenctoReal, obj.Valor, null, "N", ref msg);

            proxy.boleto.PercentualJurosDia = obj.Juros;
            proxy.boleto.PercentualMulta = obj.Multa;
            proxy.boleto.ValorJurosDia = Math.Round((obj.Valor * (obj.Juros / 100)), 2);
            proxy.boleto.ValorMulta = Math.Round((obj.Valor * (obj.Multa / 100)), 2);

            proxy.boleto.DataJuros = obj.DataVencto;
            proxy.boleto.DataMulta = obj.DataVencto;

            if (!string.IsNullOrEmpty(msg))
                throw new Exception(msg);

            proxy.FecharBoleto(ref msg);

            if (!string.IsNullOrEmpty(msg))
                throw new Exception(msg);
        }

        public static KeyValuePair<string, string> GerarBoleto(Empresa empresa, ParametroBanco prm, Cliente cli, int nossoNum, string numDoc, string carteira, DateTime vencto, decimal valor)
        {
            var bol = CriaBoleto(empresa, prm, cli, nossoNum, numDoc, carteira, vencto, valor);
            return new KeyValuePair<string, string>(bol.NossoNumeroFormatado, bol.CodigoBarra.CodigoDeBarras);
        }

        public static void GerarBoleto(Cliente cli, ParametroBanco prm, Domain.Entities.Boleto obj,string nome, string folderpath)
        {
            string msg = null;
            var proxy = GerarProxy(obj.Empresa, prm);
            AdicionarBoleto(ref proxy, obj.Cliente, obj);
            
            var bolfile = Path.Combine(folderpath, nome);
            proxy.GerarBoletos(bolfile, ref msg);
            
            if(!string.IsNullOrEmpty(msg))
                throw new Exception(msg);
        }

        public static void GerarBoletos(Empresa emp, List<Domain.Entities.Boleto> bols, string nome, string folderpath)
        {
            if (!bols.Any())
                throw new Exception("Sem boletos para gerar CNAB");

            var msg = string.Empty;

            var proxy = GerarProxy(emp, emp.ParametroBanco);

            foreach (var obj in bols)
                AdicionarBoleto(ref proxy, obj.Cliente, obj);

            var bolfile = Path.Combine(folderpath, nome);
            proxy.GerarBoletos(bolfile, ref msg);

            if (!string.IsNullOrEmpty(msg))
                throw new Exception(msg);
        }

        public static void GerarRemessa(Empresa emp, ParametroBanco prm, List<Domain.Entities.Boleto> bols, string folderpath, string filename, int sequencial)
        {
            if(!bols.Any())
                throw new Exception("Sem boletos para gerar CNAB");

            var msg = string.Empty;

            var proxy = GerarProxy(emp, prm);

            foreach (var obj in bols)
                AdicionarBoleto(ref proxy, obj.Cliente, obj);

            var path = Path.Combine(folderpath, filename);
            proxy.GerarRemessa((int) TipoArquivo.CNAB400, path, sequencial, ref msg);
            
            if (!string.IsNullOrEmpty(msg))
                throw new Exception(msg);
        }

        public static List<DadosArquivoRetornoLido> LerArquivoRetornoPeloStream(Stream stream)
        {
            var lista = new List<DadosArquivoRetornoLido>();

            var proxy = new Boleto2NetProxy();

            var banco = Banco.Instancia(237);

            var instaciaRetorno = new ArquivoRetorno(banco, TipoArquivo.CNAB400, true);
 
            var resultado = instaciaRetorno.LerArquivoRetorno(stream);

            if(resultado != null && resultado.Any())
            {
                foreach(var item in resultado)
                {
                    if (item.NumeroDocumento == null || item.NumeroDocumento.TrimStart().Trim().TrimEnd() != "")
                    {
                        if (item.ValorPago > 0)
                        {
                            lista.Add(new DadosArquivoRetornoLido()
                            {
                                NumeroDocumento = item.NumeroDocumento.TrimStart().Trim().TrimEnd(),
                                NossoNumero = item.NossoNumero,
                                DescricaoOcorrencia = item.DescricaoOcorrencia,
                                NossoNumeroFormatado = item.NossoNumeroFormatado,
                                DataCredito = item.DataCredito
                            });
                        }
                    }
                }
            }

            return lista;
        }
    }

    public class DadosArquivoRetornoLido
    {
        public string NumeroDocumento { get; set; }

        public string NossoNumero { get; set; }

        public string NossoNumeroFormatado { get; set; }

        public string DescricaoOcorrencia { get; set; }

        public DateTime DataCredito { get; set; }
    }
}