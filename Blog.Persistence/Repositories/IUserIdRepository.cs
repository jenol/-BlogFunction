using System;
using System.Threading.Tasks;

namespace Blog.Persistence.Repositories
{
    public interface IUserIdRepository
    {
        Task<Guid?> GetUserIdAsync(byte[] userName);
        Task UpsertUserIdAsync(byte[] userName, Guid userId);
    }
}