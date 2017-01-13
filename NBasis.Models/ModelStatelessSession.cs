using Serilog;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NBasis.Models
{
    public class ModelStatelessSession : IModelSession
    {
        readonly Guid _sessionId;
        readonly DbContext _context;
        readonly ILogger _log;

        public ModelStatelessSession(DbContext context)
        {
            _log = Log.Logger.ForContext<ModelStatelessSession>();

            _sessionId = Guid.NewGuid();
            _context = context;

            _log.Debug("Session created: {SessionId}", _sessionId);
            
            // enable context logging
            //if (_log.IsDebugEnabled)
            //{
            //    _Context.Database.Log = (s) =>
            //    {
            //        _log.Debug(s);
            //    };
            //}
        }            

        public Task<T> LoadAsync<T, TKey>(TKey id) where T : class, IEntity<TKey>
        {
            return _context.Set<T>().FindAsync(id);
        }        

        public void Dispose()
        {
            // nothing to cleanup.. but we'll log it anyways
            _log.Debug("Session disposed: {SessionId}", _sessionId);
        }
       
        Task<IEnumerable<TEntity>> IModelSession.FilterAsync<TEntity, TKey>(IFilter<TEntity, TKey> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query), "Must have a query to execute");
            return query.Execute(_context.Set<TEntity>());
        }

        Task<IEnumerable<TResult>> IModelSession.FilterAsync<TResult>(IDbContextFilter<TResult> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query), "Must have a query to execute");
            return query.Execute(_context);
        }        

        public DbContext Context
        {
            get
            {
                return _context;
            }
        }
    }
}
