using System;

namespace Core_Api.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Rollback();
        bool Commit();
        TRepository GetRepository<TRepository>() where TRepository : IRepository;
    }
}