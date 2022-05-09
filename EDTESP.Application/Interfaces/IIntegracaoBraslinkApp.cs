using EDTESP.Domain.Entities;
using System.Collections.Generic;

namespace EDTESP.Application.Interfaces
{
    public interface IIntegracaoBraslinkApp
    {
        bool IntegrarClienteParaBraslink(int clienteId, string caminhoBase);

        bool ExcluirClienteBraslink(int clienteId);

        List<int> RetornarClientesIntegrados();

        bool AlterarClienteParaBraslink(ClienteBraslink clienteBraslink, string caminhoBase);

        ClienteBraslink RetornarClienteBraslinkPeloId(int clienteId);
    }
}
