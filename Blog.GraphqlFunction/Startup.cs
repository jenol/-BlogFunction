using Blog.GraphqlFunction;
using Blog.GraphqlFunction.Configuration;
using Guyabano.DepenencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Blog.GraphqlFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.AddModule<IocModule>();
            builder.Services.AddSingleton<HttpTrigger>();
        }
    }
}