using EDTESP.Domain.Entities.Relatorios;
using System;
using System.Collections.Generic;

namespace EDTESP.Domain.Repositories
{
    public interface ICobrancaClienteRepository
    {
        List<CobrancaCliente> RetornarDados(int cliente, DateTime dataInicial, DateTime dataFinal, DateTime dataComparacao, int vendedorId, int numeroContrato = 0);
    }
}
