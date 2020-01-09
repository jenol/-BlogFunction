using System;
using System.Threading.Tasks;
using Blog.Persistence.Entities;
using Microsoft.WindowsAzure.Storage;

namespace Blog.Persistence.Repositories
{
    internal class UserNameRepository : RepositoryBase<UserNameEntity>, IUserNameRepository
    {
        public UserNameRepository(string appTablePrefix, CloudStorageAccount cloudStorageAccount) :
            base(appTablePrefix, cloudStorageAccount) { }

        protected override string TableName => $"{AppTablePrefix}UserName";

        public async Task<byte[]> GetUserNameAsync(Guid userId)
        {
            var entity = await RetrieveEntityUsingPointQueryAsync(
                UserNameEntity.GetPartitionKey(userId),
                userId.ToString());

            return entity?.UserName;
        }

        public async Task UpsertUserIdAsync(byte[] userName, Guid userId)
        {
            await UpsertAsync(new UserNameEntity(userId, userName));
        }
    }
}