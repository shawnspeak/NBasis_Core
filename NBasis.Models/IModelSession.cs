using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NBasis.Models
{
    public interface IModelSession : IDisposable
    {
        Task<T> LoadAsync<T, TKey>(TKey id) where T : class, IEntity<TKey>;
        
        Task<IEnumerable<TEntity>> FilterAsync<TEntity, TKey>(IFilter<TEntity,TKey> filter) where TEntity : class, IEntity<TKey>;

        Task<IEnumerable<TResult>> FilterAsync<TResult>(IDbContextFilter<TResult> filter) where TResult : class;
    }

    public interface IModelStatefullSession : IModelSession
    {     
        Task StoreAsync<T, TKey>(T item) where T : class, IEntity<TKey>;

        Task CreateOrAlterAsync<T, TKey>(TKey id, Func<T, T> createOrAlter) where T : class, IEntity<TKey>;

        Task AlterAsync<T, TKey>(TKey id, Action<T> alterItem) where T : class, IEntity<TKey>;

        Task<bool> RemoveAsync<T, TKey>(TKey id) where T : class, IEntity<TKey>;

        void Remove<T, TKey>(T item) where T : class, IEntity<TKey>;
    }

    public enum SessionState
    {
        Open,
        RolledBack,
        Committed
    }    
}
