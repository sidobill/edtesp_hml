using EDTESP.Application.Interfaces;
using EDTESP.Domain.Entities.Relatorios;
using EDTESP.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using EDTESP.Excel;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Linq;
using System.Text;

namespace EDTESP.Application
{
    public class CobrancaClienteAppService : ICobrancaClienteAppService
    {
        private readonly ICobrancaClienteRepository _cobrancaClienteRepository;

        public CobrancaClienteAppService(ICobrancaClienteRepository cobrancaClienteRepository)
        {
            _cobrancaClienteRepository = cobrancaClienteRepository;
        }

        public List<CobrancaCliente> RetornarDados(int cliente, DateTime dataInicial, DateTime dataFinal, DateTime dataComparacao, int vendedorId)
        {
            return _cobrancaClienteRepository.RetornarDados(cliente, dataInicial, dataFinal, dataComparacao, vendedorId);
        }

        public MemoryStream GerarExcel(int cliente, DateTime dataInicial, DateTime dataFinal, DateTime dataComparacao, int vendedorId)
        {
            var lista = _cobrancaClienteRepository.RetornarDados(cliente, dataInicial, dataFinal, dataComparacao, vendedorId);

            var dataSet = new DataSet("Cobranca");
            dataSet.Tables.Add(RetornarDatablePelaLista(lista));

            var excelService = new ExcelService();

            return excelService.GerarExcel(dataSet);
        }

        private DataTable RetornarDatablePelaLista(List<CobrancaCliente> cobrancas)
        {
            var data = new DataTable();
            data.Columns.Add("Contrato");
            data.Columns.Add("Cliente");
            data.Columns.Add("Documento");
            data.Columns.Add("Endereço");
            data.Columns.Add("Bairro");
            data.Columns.Add("Cidade");
            data.Columns.Add("CEP");
            data.Columns.Add("Responsavel");
            data.Columns.Add("Telefone");
            data.Columns.Add("Representante");
            data.Columns.Add("Parcela");
            data.Columns.Add("Vencimento");
            data.Columns.Add("Valor");
            data.Columns.Add("Status");

            foreach (var item in cobrancas)
            {
                data.Rows.Add(item.Numero, item.RazaoSocial, item.Documento, item.EnderecoCobr, item.BairroCobr, item.CidadeCobr, item.CepCobr, item.Responsavel, item.Telefone, item.Vendedor, (item.Parcela.ToString() + "/" + item.QuantidadeTotalParcela), item.DataVenctoReal.ToString("dd/MM/yyyy"), item.ValorParcela, RetornarStatus(item.DataVenctoReal));
            }

            return data;
        }

        private string RetornarStatus(DateTime data)
        {
            if (data < DateTime.Now.Date)
                return "Atrasado";
            else
                return "Normal";
        }

        public byte[] RetornarDadosPdf(int cliente, DateTime dataInicial, DateTime dataFinal, DateTime dataComparacao, int vendedorId)
        {
            using (var memoryStream = new MemoryStream())
            {
                var lista = _cobrancaClienteRepository.RetornarDados(cliente, dataInicial, dataFinal, dataComparacao, vendedorId);

                if (lista != null && lista.Any())
                {
                    var document = new Document(PageSize.A4, 50, 50, 60, 60);
                    var writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    var agrupamento = lista.GroupBy(g => new { g.Vendedor, g.Numero, g.RazaoSocial, g.Documento });

                    foreach (var item in agrupamento)
                    {
                        var tabela = new PdfPTable(3);
                        tabela.WidthPercentage = 100;

                        var cel = new PdfPCell(new Phrase("Contrato:"));
                        cel.Border = 0;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase(item.Key.Numero.ToString()));
                        cel.Colspan = 2;
                        cel.Border = 0;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase("Cliente:"));
                        cel.Border = 0;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase(item.Key.RazaoSocial));
                        cel.Colspan = 2;
                        cel.Border = 0;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase("CPF/CNPJ:"));
                        cel.Border = 0;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase(item.Key.Documento));
                        cel.Border = 0;
                        cel.Colspan = 2;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase("Endereço:"));
                        cel.Border = 0;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase(item.ToList().First().EnderecoCobr + ", " + item.ToList().First().NumeroCobr));
                        cel.Border = 0;
                        cel.Colspan = 2;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase("Bairro:"));
                        cel.Border = 0;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase(item.ToList().First().BairroCobr));
                        cel.Border = 0;
                        cel.Colspan = 2;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase("Cidade:"));
                        cel.Border = 0;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase(item.ToList().First().CidadeCobr));
                        cel.Border = 0;
                        cel.Colspan = 2;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase("CEP:"));
                        cel.Border = 0;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase(item.ToList().First().CepCobr));
                        cel.Border = 0;
                        cel.Colspan = 2;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase("Fones:"));
                        cel.Border = 0;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase(item.ToList().First().Telefone));
                        cel.Border = 0;
                        cel.Colspan = 2;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase("Responsável:"));
                        cel.Border = 0;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase(item.ToList().First().Responsavel));
                        cel.Border = 0;
                        cel.Colspan = 2;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase("Representante:"));
                        cel.Border = 0;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase(item.Key.Vendedor));
                        cel.Border = 0;
                        cel.Colspan = 2;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase("Cedente:"));
                        cel.Border = 0;
                        tabela.AddCell(cel);

                        cel = new PdfPCell(new Phrase(item.ToList().First().Fantasia));
                        cel.Colspan = 2;
                        cel.Border = 0;
                        tabela.AddCell(cel);

                        document.Add(tabela);

                        var parcelas = item.ToList();

                        if (parcelas != null && parcelas.Any())
                        {
                            var tabelaParcela = new PdfPTable(3);
                            tabela.WidthPercentage = 100;

                            cel = new PdfPCell(new Phrase(""));
                            cel.Border = 0;
                            cel.Colspan = 3;
                            tabelaParcela.AddCell(cel);

                            cel = new PdfPCell(new Phrase("No. Parcela"));
                            cel.Border = 0;
                            cel.HorizontalAlignment = Element.ALIGN_CENTER;
                            tabelaParcela.AddCell(cel);

                            cel = new PdfPCell(new Phrase("Vencimento"));
                            cel.Border = 0;
                            cel.HorizontalAlignment = Element.ALIGN_CENTER;
                            tabelaParcela.AddCell(cel);

                            cel = new PdfPCell(new Phrase("Valor"));
                            cel.Border = 0;
                            cel.HorizontalAlignment = Element.ALIGN_CENTER;
                            tabelaParcela.AddCell(cel);

                            var resultadoOutros = _cobrancaClienteRepository.RetornarDados(item.ToList().First().ClienteId, new DateTime(2000, 1, 1), dataInicial.AddDays(-1), DateTime.Now, 0, item.Key.Numero);

                            if (resultadoOutros != null && resultadoOutros.Any())
                                parcelas.AddRange(resultadoOutros);

                            parcelas = parcelas.OrderBy(o => o.DataVenctoReal).ToList();

                            foreach(var parc in parcelas)
                            {
                                cel = new PdfPCell(new Phrase(parc.Parcela.ToString() + "/" + parc.QuantidadeTotalParcela.ToString()));
                                cel.Border = 0;
                                cel.HorizontalAlignment = Element.ALIGN_CENTER;
                                tabelaParcela.AddCell(cel);

                                cel = new PdfPCell(new Phrase(parc.DataVenctoReal.ToString("dd/MM/yyyy")));
                                cel.Border = 0;
                                cel.HorizontalAlignment = Element.ALIGN_CENTER;
                                tabelaParcela.AddCell(cel);

                                cel = new PdfPCell(new Phrase(parc.ValorParcela.ToString("c")));
                                cel.Border = 0;
                                cel.HorizontalAlignment = Element.ALIGN_CENTER;
                                tabelaParcela.AddCell(cel);
                            }

                            document.Add(tabelaParcela);
                        }

                        document.NewPage();
                    }

                    document.Close();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
