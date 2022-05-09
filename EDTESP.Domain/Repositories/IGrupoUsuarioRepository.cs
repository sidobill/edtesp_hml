using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EDTESP.Domain.Entities;

namespace EDTESP.Domain.Repositories
{
    public interface IGrupoUsuarioRepository : IRepositoryBase<GrupoUsuario>
    {
        new IEnumerable<GrupoUsuario> List();

        new IQueryable<GrupoUsuario> Where(Expression<Func<GrupoUsuario, bool>> expression);
    }
}