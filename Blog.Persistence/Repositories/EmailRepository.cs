using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Persistence.Entities;
using Microsoft.WindowsAzure.Storage;

namespace Blog.Persistence.Repositories
{
    internal class EmailRepository : RepositoryBase<EmailEntity>, IEmailRepository
    {
        public EmailRepository(string appTablePrefix, CloudStorageAccount cloudStorageAccount) :
            base(appTablePrefix, cloudStorageAccount) { }

        protected override string TableName => $"{AppTablePrefix}Email";

        public async Task<byte[]> GetUserNameByEmailAsync(string email)
        {
            var partitionKey = EmailEntity.GetPartitionKey(email);
            var entity = await RetrieveEntityUsingPointQueryAsync(partitionKey, email);

            return entity?.UserName;
        }

        public async Task<Dictionary<string, byte[]>> GetUserNamesByEmailsAsync(IEnumerable<string> emails)
        {
            var tasksByEmail = emails.Distinct().ToDictionary(e => e, e => GetUserNameByEmailAsync(e));

            try
            {
                await Task.WhenAll(tasksByEmail.Values);
            }
            catch (Exception ex)
            {
                var o = 0;
            }

            var succeeded = tasksByEmail.Where(t => !t.Value.IsFaulted).ToArray();
            var failed = tasksByEmail.Where(t => t.Value.IsFaulted).ToArray();

            return succeeded.Where(t => t.Value.Result != null && t.Value.Result.Any())
                .ToDictionary(t => t.Key, t => t.Value.Result);
        }

        public async Task UpsertEmailAsync(byte[] userName, string email)
        {
            await UpsertAsync(new EmailEntity(userName, email));
        }
    }
}