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
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity<TEntity>
    {
        protected readonly FirstContext Db;
        protected Repository(FirstContext context)
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

        public virtual TEntity GetById(long id)
        {
            return Db.Set<TEntity>().AsNoTracking().FirstOrDefault(t => t.Deleted == false && t.Id == id);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return Db.Set<TEntity>().Where(t => t.Deleted == false).ToList();
        }

        public virtual void Remove(long id)
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
                sb.AppendLine($"Error: technic detail::: {e?.InnerException?.InnerException?.Message}");

                foreach (var eve in e.Entries)
                {
                    sb.AppendLine($"Object [{eve.Entity.GetType().Name}] in state [{eve.State}] can't be updated.");
                }
            }
        }

        public void Dispose()
        {
            Db.Dispose();
        }
    }
}