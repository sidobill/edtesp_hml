using EDTESP.Domain.Entities.Relatorios;
using System;
using System.Collections.Generic;

namespace EDTESP.Domain.Repositories
{
    public interface IComissaoIndividualRepository
    {
        List<ComissaoIndividual> RetonarDados(int vendedorId, DateTime dataInicial, DateTime dataFinal, int time = 0);
    }
}
