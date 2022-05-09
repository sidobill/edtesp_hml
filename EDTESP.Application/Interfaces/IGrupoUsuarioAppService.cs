using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Repositories;

namespace EDTESP.Application.Interfaces
{
    public interface IGrupoUsuarioAppService : IAppServiceBase<GrupoUsuario>
    {
        new IEnumerable<GrupoUsuario> List();

        new IQueryable<GrupoUsuario> Where(Expression<Func<GrupoUsuario, bool>> expression);
    }
}