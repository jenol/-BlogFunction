using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Persistence.Repositories
{
    public interface IEmailRepository
    {
        Task<byte[]> GetUserNameByEmailAsync(string email);
        Task<Dictionary<string, byte[]>> GetUserNamesByEmailsAsync(IEnumerable<string> emails);
        Task UpsertEmailAsync(byte[] userName, string email);
    }
}