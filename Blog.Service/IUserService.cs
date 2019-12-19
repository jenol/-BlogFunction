using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Service
{
    public interface IUserService
    {
        Task<User> GetUserBySecurityTokenAsync(string securityToken);
        Task<string> GetSecurityTokenAsync(string username, string password);
        Task<User> GetUserAsync(string userName);
        Task<User> GetUserAsync(Guid userId);
        Task<IEnumerable<User>> AddUsersAsync(IEnumerable<UserInput> users);
    }
}