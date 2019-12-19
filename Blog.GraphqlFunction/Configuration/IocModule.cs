using Blog.Service.Configuration;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Guyabano.DepenencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.GraphqlFunction.Configuration
{
    internal class IocModule : IIocModule
    {
        public void AddServices(IFunctionsHostBuilder builder)
        {
            builder.AddModule<ServiceModule>();

            builder.Services.AddSingleton<ISchema>(p => new BlogServiceSchema(p));
            builder.Services.AddSingleton<BlogServiceQuery>();
            builder.Services.AddSingleton<IDocumentExecuter>(p => new DocumentExecuter());
            builder.Services.AddSingleton<IDocumentWriter>(p => new DocumentWriter(true));

            builder.Services.AddSingleton<BlogServiceMutation>();
        }
    }
}