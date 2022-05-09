using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Repositories;
using EDTESP.Infrastructure.Data.Context;

namespace EDTESP.Infrastructure.Data.Repositories
{
    public class GrupoUsuarioRepository : RepositoryBase<GrupoUsuario>, IGrupoUsuarioRepository
    {
        public GrupoUsuarioRepository(IDbContexto context) : base(context)
        {
        }

        public new IEnumerable<GrupoUsuario> List()
        {
            var qry = context.GrupoUsuarios.ToList();

            qry.ForEach(x => { x.UsuarioCriador = context.Usuarios.Find(x.UsuarioCriadorId); });

            return qry.ToList();
        }

        public new IQueryable<GrupoUsuario> Where(Expression<Func<GrupoUsuario, bool>> expression)
        {
            var qry = base.Where(expression).ToList();

            qry.ForEach(x => { x.UsuarioCriador = context.Usuarios.Find(x.UsuarioCriadorId); });

            return qry.AsQueryable();
        }
    }
}