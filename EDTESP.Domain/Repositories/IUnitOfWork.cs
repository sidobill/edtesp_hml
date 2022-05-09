using System;
using EDTESP.Domain.Entities;

namespace EDTESP.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
        new void Dispose();
    }
}