using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core_Api.Domain.Core.Models;
using Core_Api.Domain.Interfaces;
using Core_Api.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Core_Api.Infra.Data.Repository
{
    public abstract class RepositoryAppUser<TEntity> : IRepositoryUser<TEntity> where TEntity : EntityUser<TEntity>
    {
        protected readonly FirstContext Db;
        protected RepositoryAppUser(FirstContext context)
        {
            Db = context;
        }

        public virtual void Add(TEntity obj)
        {            
            Db.Set<TEntity>().Add(obj);
            Save();
        }

        public virtual void Update(TEntity obj)
        {
            Db.Entry(obj).State = EntityState.Modified;
            Save();
        }

        public virtual IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> predicate)
        {
            Expression<Func<TEntity, bool>> naoDeleted = c => c.Deleted == false;
            Expression<Func<TEntity, bool>> naoDeletedAndPredicate = naoDeleted.And(predicate);
            return Db.Set<TEntity>().Where(naoDeletedAndPredicate);
        }

        public virtual TEntity GetById(Guid id)
        {
            return Db.Set<TEntity>().AsNoTracking().FirstOrDefault(t => t.Deleted == false && t.UserID == id);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return Db.Set<TEntity>().Where(t => t.Deleted == false).ToList();
        }

        public virtual void Remove(Guid id)
        {
            var obj = Db.Set<TEntity>().Find(id);
            Db.Entry(obj).State = EntityState.Deleted;
            Save();
        }

        private void Save()
        {
            try
            {
                Db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"DbUpdateException error details - {e?.InnerException?.InnerException?.Message}");

                foreach (var eve in e.Entries)
                {
                    sb.AppendLine($"Entity of type {eve.Entity.GetType().Name} in state {eve.State} could not be updated");
                }
            }
        }

        public void Dispose()
        {
            Db.Dispose();
        }
    }
}