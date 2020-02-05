using System.Threading.Tasks;

namespace Blog.Persistence.Repositories
{
    public interface IUserNameRepository
    {
        Task<byte[]> GetUserNameAsync(byte[] userId);
        Task UpsertUserIdAsync(byte[] userName, byte[] userId);
    }
}