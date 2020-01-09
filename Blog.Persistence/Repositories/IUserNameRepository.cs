using System;
using System.Threading.Tasks;

namespace Blog.Persistence.Repositories
{
    public interface IUserNameRepository
    {
        Task<byte[]> GetUserNameAsync(Guid userId);
        Task UpsertUserIdAsync(byte[] userName, Guid userId);
    }
}