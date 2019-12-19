using System;
using System.Threading.Tasks;

namespace Blog.Persistence.Repositories
{
    public interface IUserNameRepository
    {
        Task<string> GetUserNameAsync(Guid userId);
        Task UpsertUserIdAsync(string userName, Guid userId);
    }
}