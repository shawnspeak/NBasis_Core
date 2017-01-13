using Microsoft.Extensions.DependencyInjection;

namespace NBasis.Setup
{
    public interface IContainerSetup
    {
        void BuildUp(IServiceCollection container);

        void TearDown();
    }
}
