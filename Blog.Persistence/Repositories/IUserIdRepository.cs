using System;
using System.Threading.Tasks;

namespace Blog.Persistence.Repositories
{
    public interface IUserIdRepository
    {
        Task<Guid> GetUserIdAsync(string userName);
        Task UpsertUserIdAsync(string userName, Guid userId);
    }
}