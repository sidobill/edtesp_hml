using System.Collections.Generic;
using EDTESP.Domain.Entities;

namespace EDTESP.Application.Interfaces
{
    public interface IUsuarioAppService : IAppServiceBase<Usuario>
    {
        Usuario Autenticar(string login, string senha);

        IEnumerable<Permissao> ObterPermissoes(int usuarioId);
    }
}