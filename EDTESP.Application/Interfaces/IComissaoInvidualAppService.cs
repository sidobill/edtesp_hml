using EDTESP.Domain.Entities.Relatorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTESP.Application.Interfaces
{
    public interface IComissaoInvidualAppService
    {
        List<ComissaoIndividual> RetornarDados(int vendedorId, DateTime dataInicial, DateTime dataFinal);
    }
}
