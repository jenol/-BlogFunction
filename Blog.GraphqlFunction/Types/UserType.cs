using Blog.Service;
using GraphQL.Types;

namespace Blog.GraphqlFunction.Types
{
    public class UserType : ObjectGraphType<User>
    {
        public UserType()
        {
            Name = "User";

            Field(u => u.UserId, type: typeof(IdGraphType)).Description("The id of the user");
            Field(u => u.UserName).Description("The login name of the user");
            Field(u => u.Email).Description("The email of the user");
            Field(u => u.FirstName).Description("The first name of the user");
            Field(u => u.LastName).Description("The last name of the user");
        }
    }
}