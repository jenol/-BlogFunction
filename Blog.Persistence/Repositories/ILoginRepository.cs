using System.Threading.Tasks;
using Blog.Persistence.Entities;

namespace Blog.Persistence.Repositories
{
    public interface ILoginRepository
    {
        Task<LoginEntity> GetLoginAsync(string loginUserName, byte[] loginPassword);

        Task<LoginEntity> UpsetLoginAsync(string loginUserName, byte[] loginPassword);
    }
}