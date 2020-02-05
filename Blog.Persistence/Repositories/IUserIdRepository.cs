using System.Threading.Tasks;

namespace Blog.Persistence.Repositories
{
    public interface IUserIdRepository
    {
        Task<byte[]> GetUserIdAsync(byte[] userName);
        Task UpsertUserIdAsync(byte[] userName, byte[] userId);
    }
}