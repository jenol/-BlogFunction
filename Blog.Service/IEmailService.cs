using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Service {
    public interface IEmailService {
        Task<Dictionary<string, string>> GetUserNamesByEmailsAsync(IEnumerable<string> emailTexts);
        Task UpsertEmailAsync(string userName, string emailText);
    }
}