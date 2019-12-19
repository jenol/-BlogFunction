using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

namespace Guyabano.DepenencyInjection
{
    public static class BuilderExtensions
    {
        public static void AddModule<T>(this IFunctionsHostBuilder builder) where T : IIocModule
        {
            builder.AddModule(Activator.CreateInstance<T>());
        }

        public static void AddModule(this IFunctionsHostBuilder builder, IIocModule module)
        {
            module.AddServices(builder);
        }
    }
}