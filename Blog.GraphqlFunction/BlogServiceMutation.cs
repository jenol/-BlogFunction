using System;
using System.Collections.Generic;
using System.Linq;
using Blog.GraphqlFunction.Types;
using Blog.Service;
using Blog.Service.Contracts;
using Blog.Service.Validation;
using GraphQL;
using GraphQL.Types;

namespace Blog.GraphqlFunction
{
    public class BlogServiceMutation : ObjectGraphType
    {
        public BlogServiceMutation(IUserService userService)
        {
            FieldAsync<ListGraphType<UserInputResultType>>(
                "createUsers",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ListGraphType<NonNullGraphType<UserInputType>>>> {Name = "users"}
                ),
                resolve: async context =>
                {
                    var users = context.GetArgument<IEnumerable<UserImport>>("users");
                    var results = new List<UserImportOperationOutcome>();

                    try
                    {
                        var p = await userService.AddUsersAsync(users.ToArray());

                        foreach (var user in p)
                        {
                            if (user.IsSuccess)
                            {
                                results.Add(user);
                            }
                            else
                            {
                                context.Errors.Add(new UserInputError(user));
                            }
                        }

                        return results;

                    }
                    catch (UserValidatorException ex)
                    {
                        throw new ExecutionError(ex.Message, ex);
                    }
                });
        }
    }
}