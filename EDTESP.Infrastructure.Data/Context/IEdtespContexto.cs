using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Entities.Edtesp;

namespace EDTESP.Infrastructure.Data.Context
{
    public interface IEdtespContexto
    {
        IDbSet<ClientesEdtesp> ClientesEdtesp { get; set; }
        IDbSet<ClassificadosEdtesp> ClassificadosEdtesp { get; set; }

        IDbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity obj) where TEntity : class;
        
        int SaveChanges();
        void Dispose();
    }
}