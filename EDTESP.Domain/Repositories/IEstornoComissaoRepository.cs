using EDTESP.Domain.Entities.Relatorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTESP.Domain.Repositories
{
    public interface IEstornoComissaoRepository
    {
        List<EstornoComissao> RetornarDados(int vendedor, DateTime dataInicial, DateTime dataFinal, DateTime dataComparacaoVencimento, int time = 0);
    }
}
