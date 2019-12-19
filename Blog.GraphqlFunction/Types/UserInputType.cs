using Blog.Service;
using GraphQL.Types;

namespace Blog.GraphqlFunction.Types
{
    public class UserInputType : InputObjectGraphType<UserInput>
    {
        public UserInputType()
        {
            Name = "UserInput";

            Field<NonNullGraphType<StringGraphType>>("userName");

            Field(u => u.UserName).Name("userName").Description("The login name of the user");
            Field(u => u.Email).Description("The email of the user");
            Field(u => u.FirstName).Description("The first name of the user");
            Field(u => u.LastName).Description("The last name of the user");
            Field(u => u.Password).Description("The password of the user");
        }
    }
}