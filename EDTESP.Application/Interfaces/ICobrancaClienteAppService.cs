using EDTESP.Domain.Entities.Relatorios;
using System;
using System.Collections.Generic;
using System.IO;

namespace EDTESP.Application.Interfaces
{
    public interface ICobrancaClienteAppService
    {
        List<CobrancaCliente> RetornarDados(int cliente, DateTime dataInicial, DateTime dataFinal, DateTime dataComparacao, int vendedorId);

        MemoryStream GerarExcel(int cliente, DateTime dataInicial, DateTime dataFinal, DateTime dataComparacao, int vendedorId);

        byte[] RetornarDadosPdf(int cliente, DateTime dataInicial, DateTime dataFinal, DateTime dataComparacao, int vendedorId);
    }
}
