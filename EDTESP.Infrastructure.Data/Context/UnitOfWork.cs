using System;
using EDTESP.Domain.Entities;
using EDTESP.Domain.Repositories;
using EDTESP.Infrastructure.Data.Repositories;

namespace EDTESP.Infrastructure.Data.Context
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private IDbContexto _context;
        private bool _disposed;

        public UnitOfWork(IDbContexto contexto)
        {
            _context = contexto;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed)
                if(disposing)
                    _context.Dispose();

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}