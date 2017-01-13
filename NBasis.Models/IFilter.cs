using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NBasis.Models
{
    //public interface IQueryOne<T, TKey> where T : class, IEntity<TKey>
    //{
    //    T Execute(IQueryable<T> queryable);
    //}

    //public interface IFilter<T, TKey> where T : class, IEntity<TKey>
    //{
    //    IEnumerable<T> Execute(IQueryable<T> queryable);
    //}

    public interface IFilter<T, TKey> where T : class, IEntity<TKey>
    {
        Task<IEnumerable<T>> Execute(IQueryable<T> queryable);
    }

    public interface IDbContextFilter<T> where T : class
    {
        Task<IEnumerable<T>> Execute(DbContext context);
    }

    //public interface IMappingQuery<TFrom, TTo, TKey> where TFrom : class, IEntity<TKey> where TTo : class
    //{
    //    IEnumerable<TTo> Execute(IQueryable<TFrom> queryable);
    //}

    //public interface IMappingAsyncQuery<TFrom, TTo, TKey> where TFrom : class, IEntity<TKey> where TTo : class
    //{
    //    Task<IEnumerable<TTo>> Execute(IQueryable<TFrom> queryable);
    //}
    
    //public interface IDatabaseQuery<T> where T : class
    //{
    //    Task<IEnumerable<T>> Execute(DbContext context);
    //}
}
