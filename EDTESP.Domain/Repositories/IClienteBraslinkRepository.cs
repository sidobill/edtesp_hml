using EDTESP.Domain.Entities;
using System.Collections.Generic;

namespace EDTESP.Domain.Repositories
{
    public interface IClienteBraslinkRepository
    {
        bool Inserir(ClienteBraslink clienteBraslink);

        bool ExcluirClienteBraslink(int clienteId);

        List<int> RetornarClientesIntegrados();

        bool Atualizar(ClienteBraslink clienteBraslink);

        ClienteBraslink RetornarPeloClienteId(int clienteId);
    }
}
