using System;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.GraphqlFunction
{
    public class BlogServiceSchema : Schema
    {
        public BlogServiceSchema(IServiceProvider provider)
        {
            Query = provider.GetRequiredService<BlogServiceQuery>();
            Mutation = provider.GetRequiredService<BlogServiceMutation>();
        }
    }
}