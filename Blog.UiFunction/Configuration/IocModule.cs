using System.Linq;
using Blog.Service.Configuration;
using Guyabano.DepenencyInjection;
using Guyabano.DepenencyInjection.Mvc;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

namespace Blog.UiFunction.Configuration
{
    internal class IocModule : IIocModule
    {
        public void AddServices(IFunctionsHostBuilder builder)
        {
            builder.AddModule<ServiceModule>();
            builder.RegisterControllers(typeof(IocModule).Assembly);

            var views = new[]
            {
                "_header",
                "_footer",
                "index",
                "thankyou",
                "login",
                "admin",
                "error_prod"
            };

            builder.RegisterViews(typeof(HttpTrigger).Assembly,
                views.Select(n => (n, $"Views/{n}.liquid")));
        }
    }
}