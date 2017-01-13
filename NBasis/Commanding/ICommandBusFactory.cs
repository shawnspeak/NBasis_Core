using NBasis.Container;

namespace NBasis.Commanding
{
    public interface ICommandBusFactory
    {
        ICommandBus GetCommandBus(IContainer container);
    }
}
