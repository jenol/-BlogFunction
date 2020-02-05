using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Service.Contracts;

namespace Blog.Service
{
    public interface IUserService
    {
        Task<User> GetUserAsync(string userName);
        Task<User> GetUserAsync(Guid userId);
        Task<IEnumerable<UserImportOperationOutcome>> AddUsersAsync(UserImport[] users);
    }
}