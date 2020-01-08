using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Persistence.Repositories
{
    public interface IEmailRepository
    {
        Task<string> GetUserNameByEmailAsync(string email);
        Task<Dictionary<string, string>> GetUserNamesByEmailsAsync(IEnumerable<string> emails);
        Task UpsertEmailAsync(string userName, string email);
    }
}