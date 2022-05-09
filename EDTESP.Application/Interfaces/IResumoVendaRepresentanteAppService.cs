using EDTESP.Domain.Entities.Relatorios;
using System;
using System.Collections.Generic;

namespace EDTESP.Application.Interfaces
{
    public interface IResumoVendaRepresentanteAppService
    {
        List<ResumoVendaRepresentante> RetornarDados(int vendedor, DateTime dataInicial, DateTime dataFinal);
    }
}
