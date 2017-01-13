using Serilog;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NBasis.Models
{
    public class ModelStatefullSession : IModelStatefullSession
    {
        readonly Guid _sessionId;
        readonly DbContext _context;
        readonly IDbContextTransaction _transaction;
        readonly ILogger _log;

        private SessionState _state = SessionState.Open;

        public ModelStatefullSession(DbContext context, IUnitOfWork unitOfWork, bool useTransactions = true, bool logSql = false)
        {
            _log = Log.Logger.ForContext<ModelStatefullSession>();

            _sessionId = Guid.NewGuid();
            _context = context;
            _log.Debug("Session created: {SessionId} Transaction: {WithTransaction}", _sessionId, useTransactions ? "yes" : "no");

            // enable context logging
            if (logSql)
            {
                //_Context.Database.
                //    _Context.Database.Log = (s) =>
                //    {
                //        _log.Debug(s);
                //    };
            }

            if (useTransactions)
                _transaction = _context.Database.BeginTransaction();

            unitOfWork.AddWork(async () =>
            {
                try
                {
                    await _context.SaveChangesAsync();

                    if (_transaction != null)
                        _transaction.Commit();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    _state = SessionState.Committed;
                }

            }, WorkPosition.Transaction);
        }
        
        public Task<T> LoadAsync<T, TKey>(TKey id) where T : class, IEntity<TKey>
        {
            CheckTransaction();
            return _context.Set<T>().FindAsync(id);
        }

        public async Task StoreAsync<T, TKey>(T item) where T : class, IEntity<TKey>
        {
            CheckTransaction();
            await _context.Set<T>().AddAsync(item);
        }

        public async Task CreateOrAlterAsync<T, TKey>(TKey id, Func<T, T> createOrAlter) where T : class, IEntity<TKey>
        {
            if (createOrAlter == null)
                throw new ArgumentNullException("createOrAlter", "Must have an Create or alter item action to create items");

            T item = await LoadAsync<T, TKey>(id);
            if (item == null)
            {
                item = createOrAlter(null as T);
                await StoreAsync<T, TKey>(item);
            }
            else
            {
                createOrAlter(item);
            }
        }

        public async Task AlterAsync<T, TKey>(TKey id, Action<T> alterAction) where T : class, IEntity<TKey>
        {
            if (alterAction == null)
                throw new ArgumentNullException("alterAction", "Must have an Alter item action to alter items");

            T item = await LoadAsync<T, TKey>(id);
            if (item != null)
            {
                alterAction(item);
            }
        }     

        public async Task<bool> RemoveAsync<T, TKey>(TKey id) where T : class, IEntity<TKey>
        {
            T item = await LoadAsync<T, TKey>(id);
            if (item != null)
            {
                _context.Set<T>().Remove(item);
                return true;
            }

            return false;
        }

        public void Remove<T, TKey>(T item) where T : class, IEntity<TKey>
        {
            if (item == null) throw new ArgumentNullException("Must have a model entity to remove");
            CheckTransaction();

            _context.Set<T>().Remove(item);
        }

        private void CheckTransaction()
        {
            switch (_state)
            {
                case SessionState.Committed: throw new Exception("Statefull session was already committed");
                case SessionState.RolledBack: throw new Exception("Statefull session was already rolled back");
                default:
                    break;
            }
        }

        //public void Rollback()
        //{
        //    CheckTransaction();

        //    try
        //    {
        //        _Transaction.Rollback();
        //    }
        //    finally
        //    {
        //        _state = SessionState.RolledBack;
        //    }
        //}

        public void Commit()
        {
            CheckTransaction();

            try
            {
                if (_transaction != null)
                    _transaction.Commit();
            }
            finally
            {
                _state = SessionState.Committed;
            }
        }

        public void Dispose()
        {
            //Dispose(true);
            //GC.SuppressFinalize(this);
        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    //if (disposing)
        //    //{
        //    //    _Transaction.Dispose();
        //    //}
        //    _log.DebugFormat("Session disposed: {0}", _SessionId);
        //}

        internal virtual void InternalDispose()
        {
            if (_transaction != null)
                _transaction.Dispose();
            _context.Dispose();
            _log.Debug("Session disposed: {SessionId}", _sessionId);
        }

        Task<IEnumerable<TEntity>> IModelSession.FilterAsync<TEntity, TKey>(IFilter<TEntity, TKey> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query), "Must have a query to execute");
            CheckTransaction();
            return query.Execute(_context.Set<TEntity>());
        }

        Task<IEnumerable<TResult>> IModelSession.FilterAsync<TResult>(IDbContextFilter<TResult> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query), "Must have a query to execute");
            CheckTransaction();
            return query.Execute(_context);
        }       

        public DbContext Context
        {
            get
            {
                return _context;
            }
        }

        public SessionState State
        {
            get { return _state; }
        }
    }
}
