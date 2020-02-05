using System.Collections.Generic;
using Blog.Service.Contracts;
using GraphQL;

namespace Blog.GraphqlFunction.Types
{
    public class UserInputError : ExecutionError
    {
        public UserInputError(UserImportOperationOutcome userImportOperationOutcome) : base("User input error",
            new Dictionary<string, object>
            {
                {"importOperationId", userImportOperationOutcome.ImportOperationId},
                {"userId", userImportOperationOutcome.UserId},
                {"isSuccess", userImportOperationOutcome.IsSuccess},
                {"notes", userImportOperationOutcome.Notes}
            }) { }
    }
}