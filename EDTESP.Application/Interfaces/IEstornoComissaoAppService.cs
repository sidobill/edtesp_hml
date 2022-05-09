using EDTESP.Domain.Entities.Relatorios;
using System;
using System.Collections.Generic;

namespace EDTESP.Application.Interfaces
{
    public interface IEstornoComissaoAppService
    {
        List<EstornoComissao> RetornarDados(int vendedor, DateTime dataInicial, DateTime dataFinal, DateTime dataComparacaoVencimento);
    }
}
