using System;

namespace NBasis.Models
{
    public interface IModelSessionFactory : IDisposable
    {
        IModelStatefullSession OpenSession();

        IModelSession OpenStatelessSession();
    }
}
