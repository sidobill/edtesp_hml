using EDTESP.Domain.Entities.Relatorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTESP.Domain.Repositories
{
    public interface IResumoVendaGeralRepository
    {
        List<ResumoVendaGeral> RetornarDados(int vendedor, DateTime dataInicial, DateTime dataFinal, int time = 0);
    }
}
