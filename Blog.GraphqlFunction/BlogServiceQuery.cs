using System;
using Blog.GraphqlFunction.Types;
using Blog.Service;
using GraphQL.Types;

namespace Blog.GraphqlFunction
{
    public class BlogServiceQuery : ObjectGraphType<object>
    {
        public BlogServiceQuery(IUserService userService)
        {
            Name = "Query";

            Field<UserType>(
                "userByUserName",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>>
                    {Name = "userName", Description = "the login name of the user"}),
                resolve: context => userService.GetUserAsync(context.GetArgument<string>("userName")));

            Field<UserType>(
                "userByUserId",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>>
                    {Name = "userId", Description = "the id of the user"}),
                resolve: context => userService.GetUserAsync(context.GetArgument<Guid>("userId")));
        }
    }
}