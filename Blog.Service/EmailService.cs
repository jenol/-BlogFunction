using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Persistence.Repositories;
using Blog.Service.DomainObjects;

namespace Blog.Service
{
    internal class EmailService : ServiceBase, IEmailService
    {
        private readonly IEmailRepository _emailRepository;

        public EmailService(string encryptionKey, string salt, IEmailRepository emailRepository) :
            base(encryptionKey, salt) => _emailRepository = emailRepository;

        public async Task<Dictionary<string, string>> GetUserNamesByEmailsAsync(IEnumerable<string> emailTexts)
        {
            var encryptedEmails = new List<string>();

            foreach (var emailText in emailTexts)
            {
                if (!Email.TryParse(emailText, out var email))
                {
                    throw new Exception($"Invalid email: {emailText}");
                }

                encryptedEmails.Add(EncryptEmail(email).ToString());
            }

            return (await _emailRepository.GetUserNamesByEmailsAsync(encryptedEmails)).ToDictionary(k => k.Key,
                k => GetDecryptedText(k.Value));
        }

        public EncryptedEmail EncryptEmail(Email email) => new EncryptedEmail(GetEncryptedText(email.ToString()));
        public Email DecryptEmail(EncryptedEmail email) => new Email(GetDecryptedText(email.ToString()));

        public async Task UpsertEmailAsync(byte[] userName, Email email)
        {
            await _emailRepository.UpsertEmailAsync(userName, EncryptEmail(email).ToString());
        }
    }
}