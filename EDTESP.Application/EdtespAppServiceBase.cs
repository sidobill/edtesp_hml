using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EDTESP.Application.Interfaces;
using EDTESP.Domain.Repositories;

namespace EDTESP.Application
{
    public class EdtespAppServiceBase<TEntity> : IEdtespAppServiceBase<TEntity> where TEntity : class
    {
        protected IUnitOfWork UnitOfWork;
        protected IEdtespRepositoryBase<TEntity> RepoBase;

        public EdtespAppServiceBase(IUnitOfWork unitOfWork, IEdtespRepositoryBase<TEntity> repobase)
        {
            UnitOfWork = unitOfWork;
            RepoBase = repobase;
        }

        public void Insert(TEntity obj)
        {
            RepoBase.Insert(obj);
            UnitOfWork.Save();
        }

        public void AddOrUpdate(TEntity obj)
        {
            RepoBase.AddOrUpdate(obj);
            UnitOfWork.Save();
        }

        public void AddOrUpdate(IEnumerable<TEntity> objs)
        {
            RepoBase.AddOrUpdate(objs);
            UnitOfWork.Save();
        }

        public void Update(TEntity obj)
        {
            RepoBase.Update(obj);
            UnitOfWork.Save();
        }

        public void Delete(TEntity obj)
        {
            RepoBase.Delete(obj);
            UnitOfWork.Save();
        }

        public void DeleteMany(IEnumerable<TEntity> objs)
        {
            RepoBase.DeleteMany(objs);
            UnitOfWork.Save();
        }

        public TEntity Get(object id)
        {
            return RepoBase.Find(id);
        }

        public TEntity Get(object id, params Expression<Func<TEntity, object>>[] includes)
        {
            return RepoBase.Find(id, includes);
        }

        public IEnumerable<TEntity> List()
        {
            return RepoBase.List();
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            return RepoBase.Where(expression);
        }
    }
}