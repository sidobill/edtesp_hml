using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using EDTESP.Domain.Repositories;
using EDTESP.Infrastructure.Data.Context;

namespace EDTESP.Infrastructure.Data.Repositories
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity>, IDisposable where TEntity : class
    {
        internal readonly IDbContexto context;
        internal IDbSet<TEntity> dbSet;

        private bool _disposed;

        public RepositoryBase(IDbContexto contexto)
        {
            context = contexto;
            dbSet = context.Set<TEntity>();
        }

        public void Insert(TEntity obj)
        {
            dbSet.Add(obj);
        }

        public void Update(TEntity obj)
        {
            dbSet.Attach(obj);
            context.Entry(obj).State = EntityState.Modified;
        }

        public void AddOrUpdate(TEntity obj)
        {
            dbSet.AddOrUpdate(obj);
        }

        public void AddOrUpdate(IEnumerable<TEntity> objs)
        {
            dbSet.AddOrUpdate(objs.ToArray());
        }

        public void Delete(TEntity obj)
        {
            dbSet.Remove(obj);
        }

        public void DeleteMany(IEnumerable<TEntity> objs)
        {
            foreach (var obj in objs)
                dbSet.Remove(obj);
        }

        public TEntity Find(object id)
        {
            return dbSet.Find(id);
        }

        public TEntity Find(object id, params Expression<Func<TEntity, object>>[] includes)
        {
            var lst = new System.Collections.Generic.List<string>();

            foreach (var inc in includes)
            {
                var bd = inc.Body as MemberExpression;
                if(bd == null)
                    throw new Exception("Includes inválidos");

                lst.Add(bd.Member.Name);
            }

            lst.ForEach(x => dbSet.Include(x));
            return dbSet.Find(id);
        }

        public IEnumerable<TEntity> List()
        {
            return dbSet.ToList();
        }
        
        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            return dbSet.Where(expression);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed)
                if(disposing)
                    context.Dispose();

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}