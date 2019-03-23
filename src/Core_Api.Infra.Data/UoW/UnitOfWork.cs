using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Ninject;
using Ninject.Parameters;
using System;
using System.Threading;
using Core_Api.Domain.Interfaces;
using Core_Api.Infra.Data.Context;

using Core_Api.Infra.Data.UoW.Singleton;

namespace Core_Api.Infra.Data.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private FirstContext _context;
        private ContextManager _contextManager { get; set; }
        private readonly IServiceProvider _servicesProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private bool rollback = false;
        private bool disposed = false;
        private bool isFirst = true;

        public UnitOfWork() { }

        public UnitOfWork(IHttpContextAccessor httpContextAccessor, IServiceProvider servicesProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _servicesProvider = servicesProvider;

            if (_httpContextAccessor.HttpContext != null)
            {
                _contextManager = _servicesProvider.GetService<ContextManager>();  /* only with Ninject */
                _context = _contextManager.Context;
            }
            /// USE SINGLETON (VALIDATE WITH ASYNC)
            else
            {
                int ThreadId = Thread.CurrentThread.ManagedThreadId;
                SynchronizationContext aaaa = SynchronizationContext.Current;

                /// VERIFY IF ALREADY EXISTS AN OPENED CONTEXT FOR THE CURRENT OPERATION
                if (UnitOfWorkContexManager.ContextOpen(ThreadId))
                {
                    _context = (FirstContext)UnitOfWorkContexManager.GetContext(ThreadId);
                    isFirst = false;
                }
                /// IF NO, THEN CREATE NEW INSTANCE AND OPEN THE TRANSACTION
                else
                {
                    _context = new FirstContext();
                    _context.Database.BeginTransaction();
                    UnitOfWorkContexManager.AddContext(_context, ThreadId);
                }
            }
        }

        public TRepository GetRepository<TRepository>() where TRepository : IRepository
        {
            var repository = _servicesProvider.GetService(typeof(TRepository)).GetType();
            IKernel kernel = new StandardKernel();
            var _param = new ConstructorArgument("context", _context);
            
            var repo = kernel.Get(repository, _param);
            
            return (TRepository)repo;
        }


        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_httpContextAccessor.HttpContext != null)
                    {
                        if (_context.Database.CurrentTransaction != null && !rollback && _contextManager.IsFirst)
                        {
                            try
                            {
                                _context.Database.CurrentTransaction.Commit();
                                
                            }
                            catch
                            {
                                _context.Database.CurrentTransaction.Rollback();
                                
                                throw;
                            }
                            finally
                            {
                                if (_contextManager.IsFirst)
                                {
                                    _context.Dispose();
                                }
                            }
                        }
                    }
                    else
                    {
                        bool isDisposed = false;
                        try
                        {
                            var id = _context.Database.CurrentTransaction.TransactionId;
                        }
                        catch
                        {
                            isDisposed = true;
                        }

                        if (!isDisposed && _context.Database.CurrentTransaction != null && !rollback && isFirst)
                        {
                            try
                            {
                                _context.Database.CurrentTransaction.Commit();
                            }
                            catch
                            {
                                _context.Database.CurrentTransaction.Rollback();
                                throw;
                            }
                            finally
                            {
                                if (isFirst)
                                {
                                    _context.Dispose();
                                    UnitOfWorkContexManager.RemoveContext(Thread.CurrentThread.ManagedThreadId);
                                }
                            }
                        }
                    }


                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            if (_httpContextAccessor.HttpContext != null)
            {
                /// REMOVES AN ITEM FROM ARRAY
                var remove = _contextManager.RemoveContex;
            }
            else
            {
                if (isFirst)
                    UnitOfWorkContexManager.RemoveContext(Thread.CurrentThread.ManagedThreadId);
            }
        }

        public void Rollback()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                if (_contextManager.IsFirst)
                {
                    if (_context.Database.CurrentTransaction != null)
                    {
                        _context.Database.RollbackTransaction();
                        rollback = true;
                    }
                }
            }
            else
            {
                if (_context.Database.CurrentTransaction != null && isFirst)
                {
                    _context.Database.CurrentTransaction.Rollback();
                    UnitOfWorkContexManager.RemoveContext(Thread.CurrentThread.ManagedThreadId);
                    rollback = true;
                }
            }

        }

        public bool Commit()
        {
            throw new NotImplementedException();
        }
    }
}