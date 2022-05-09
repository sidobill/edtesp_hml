using EDTESP.Domain.Entities.Relatorios;
using System;
using System.Collections.Generic;

namespace EDTESP.Application.Interfaces
{
    public interface IAdiantamentoSalarialAppService
    {
        List<AdiantamentoSalarial> RetornarDados(int vendedorId, DateTime dataInicial, DateTime dataFinal);
    }
}
