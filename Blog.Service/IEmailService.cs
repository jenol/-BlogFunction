using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Service.DomainObjects;

namespace Blog.Service
{
    public interface IEmailService
    {
        Task<Dictionary<string, string>> GetUserNamesByEmailsAsync(IEnumerable<string> emailTexts);
        Task UpsertEmailAsync(byte[] userName, Email email);

        EncryptedEmail EncryptEmail(Email email);
        Email DecryptEmail(EncryptedEmail email);
    }
}