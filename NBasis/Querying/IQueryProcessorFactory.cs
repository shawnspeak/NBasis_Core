using NBasis.Container;

namespace NBasis.Querying
{
    public interface IQueryProcessorFactory
    {
        IQueryProcessor GetQueryProcessor(IContainer container);
    }
}
