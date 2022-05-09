using EDTESP.Domain.Entities.Relatorios;
using System;
using System.Collections.Generic;

namespace EDTESP.Domain.Repositories
{
    public interface IAdiantamentoSalarialRepository
    {
        List<AdiantamentoSalarial> RetonarDados(int vendedorId, DateTime dataInicial, DateTime dataFinal, int time = 0);
    }
}
