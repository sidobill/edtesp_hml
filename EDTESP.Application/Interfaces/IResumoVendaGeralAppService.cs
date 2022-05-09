using EDTESP.Domain.Entities.Relatorios;
using System;
using System.Collections.Generic;

namespace EDTESP.Application.Interfaces
{
    public interface IResumoVendaGeralAppService
    {
        List<ResumoVendaGeral> RetornarDados(int vendedor, DateTime dataInicial, DateTime dataFinal);
    }
}
