using System.Threading.Tasks;
using Blog.Persistence.Entities;

namespace Blog.Persistence.Repositories
{
    public interface ILoginRepository
    {
        Task<LoginEntity> GetLoginAsync(byte[] userName, byte[] password);

        Task<LoginEntity> UpsetLoginAsync(byte[] userName, byte[] password);
    }
}