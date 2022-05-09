using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EDTESP.Application.Interfaces
{
    public interface IAppServiceBase<TEntity>  where TEntity : class
    {
        void Insert(TEntity obj);

        void AddOrUpdate(TEntity obj);

        void AddOrUpdate(IEnumerable<TEntity> objs);

        void Update(TEntity obj);

        void Delete(TEntity obj);

        void DeleteMany(IEnumerable<TEntity> objs);

        TEntity Get(object id);

        TEntity Get(object id, params Expression<Func<TEntity, object>>[] includes);

        IEnumerable<TEntity> List();

        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression);
    }
}