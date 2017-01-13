using System.Threading.Tasks;

namespace NBasis.Querying
{
    public interface IHandleQueries
    {
    }

    public interface IHandleQueries<TQuery, TResult> : IHandleQueries where TQuery : IQuery<TResult>
    {
        Task<TResult> Handle(IQueryHandlingContext<TQuery, TResult> handlingContext);
    }
}
