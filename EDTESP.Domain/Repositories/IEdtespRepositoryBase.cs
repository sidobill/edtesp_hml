using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EDTESP.Domain.Repositories
{
    public interface IEdtespRepositoryBase<TEntity> : IDisposable where TEntity : class
    {
        void Insert(TEntity obj);

        void AddOrUpdate(TEntity obj);

        void AddOrUpdate(IEnumerable<TEntity> objs);

        void Update(TEntity obj);

        void Delete(TEntity obj);

        void DeleteMany(IEnumerable<TEntity> objs);

        TEntity Find(object id);

        TEntity Find(object id, params Expression<Func<TEntity, object>>[] includes);

        IEnumerable<TEntity> List();

        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression);
    }
}