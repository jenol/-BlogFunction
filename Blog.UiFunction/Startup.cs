using Blog.UiFunction;
using Blog.UiFunction.Configuration;
using Guyabano.DepenencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Blog.UiFunction
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