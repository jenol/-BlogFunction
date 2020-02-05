using Blog.Service.Contracts;
using GraphQL.Types;

namespace Blog.GraphqlFunction.Types
{
    public class UserInputResultType : ObjectGraphType<UserImportOperationOutcome>
    {
        public UserInputResultType()
        {
            Name = "UserInputResult";

            Field(u => u.ImportOperationId).Name("importOperationId").Description("The importOperationId user");
            Field(u => u.UserId, type: typeof(IdGraphType), nullable: true).Name("userId").Description("The login name of the user");
            Field(u => u.IsSuccess).Description("The email of the user");
            Field(u => u.Notes).Description("The first name of the user");
        }
    }
}