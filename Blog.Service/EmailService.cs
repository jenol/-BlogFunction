using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Persistence.Repositories;
using Blog.Service.DomainObjects;

namespace Blog.Service
{
    internal class EmailService : IEmailService
    {
        private const string EncryptionKey = "abc123";
        private readonly IEmailRepository _emailRepository;

        public EmailService(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public Task<Dictionary<string, string>> GetUserNamesByEmailsAsync(IEnumerable<string> emailTexts)
        {
            var encryptedEmails = new List<string>();

            foreach (var emailText in emailTexts)
            {
                if (!Email.TryParse(emailText, out var email))
                {
                    throw new Exception($"Invalid email: {emailText}");
                }

                encryptedEmails.Add(email.EncryptedEmail(EncryptionKey));
            }

            return _emailRepository.GetUserNamesByEmailsAsync(encryptedEmails);
        }

        public async Task UpsertEmailAsync(string userName, string emailText)
        {
            if (Email.TryParse(emailText, out var email))
            {
                await UpsertEmailAsync(userName, email);
            }

            throw new Exception($"Invalid email: {emailText}");
        }

        internal async Task UpsertEmailAsync(string userName, Email email)
        {
            await _emailRepository.UpsertEmailAsync(userName, email.EncryptedEmail(EncryptionKey));
        }
    }
}
