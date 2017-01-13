using System;

namespace NBasis.Container
{
    public interface IContainer
    {
        object Resolve(Type type);

        T Resolve<T>();
    }
}
