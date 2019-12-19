using Microsoft.Azure.Functions.Extensions.DependencyInjection;

namespace Guyabano.DepenencyInjection
{
    public interface IIocModule
    {
        void AddServices(IFunctionsHostBuilder builder);
    }
}