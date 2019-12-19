using System.Collections.Generic;
using Blog.GraphqlFunction.Types;
using Blog.Service;
using GraphQL.Types;

namespace Blog.GraphqlFunction
{
    public class BlogServiceMutation : ObjectGraphType
    {
        public BlogServiceMutation(IUserService userService)
        {
            Field<ListGraphType<UserType>>(
                "createUsers",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ListGraphType<NonNullGraphType<UserInputType>>>> {Name = "users"}
                ),
                resolve: context =>
                {
                    var users = context.GetArgument<IEnumerable<UserInput>>("users");
                    return userService.AddUsersAsync(users).Result;
                });
        }
    }
}