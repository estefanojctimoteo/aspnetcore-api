using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Core_Api.Domain.Interfaces
{
    public interface IRepositoryUser { }

    public interface IRepositoryUser<TEntity> : IRepositoryUser
    {
        void Add(TEntity obj);
        TEntity GetById(Guid id);
        IEnumerable<TEntity> GetAll();
        void Update(TEntity obj);
        void Remove(Guid id);
        IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> predicate);        
    }
}