using System.Threading.Tasks;

namespace NBasis.Querying
{
    public interface IQueryProcessor
    {
        /// <summary>
        /// Execute a query
        /// </summary>
        /// <returns></returns>
        Task<TResult> ExecuteAsync<TQuery, TResult>(Envelope<TQuery> query) where TQuery : IQuery<TResult>;        
    }
}
