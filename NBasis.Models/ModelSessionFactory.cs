using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NBasis.Configuration;

namespace NBasis.Models
{
    public class ModelSessionFactory<TContext> : IModelSessionFactory where TContext : DbContext, new()
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IConfigurationRoot _configuration;

        public ModelSessionFactory(IUnitOfWork unitOfWork, IConfigurationRoot configuration)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        private ModelStatefullSession _StatefullSession;

        private TContext _StatelessSession;

        public IModelStatefullSession OpenSession()
        {
            if (_StatefullSession == null)
                _StatefullSession = new ModelStatefullSession(new TContext(), _unitOfWork, _configuration.NBasisConfig().GetBool("UseTransactions", true));
            return _StatefullSession;
        }

        public IModelSession OpenStatelessSession()
        {
            if (_StatelessSession == null)
            {
                _StatelessSession = new TContext();
                _StatelessSession.ChangeTracker.AutoDetectChangesEnabled = false;
            }
            return new ModelStatelessSession(_StatelessSession);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_StatefullSession != null)
                    _StatefullSession.InternalDispose();

                if (_StatelessSession != null)
                    _StatelessSession.Dispose();
            }
        }
    }
}
