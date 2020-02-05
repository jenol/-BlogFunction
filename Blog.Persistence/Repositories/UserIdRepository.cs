using System.Threading.Tasks;
using Blog.Persistence.Entities;
using Microsoft.WindowsAzure.Storage;

namespace Blog.Persistence.Repositories
{
    internal class UserIdRepository : RepositoryBase<UserIdEntity>, IUserIdRepository
    {
        public UserIdRepository(string appTablePrefix, CloudStorageAccount cloudStorageAccount) :
            base(appTablePrefix, cloudStorageAccount) { }

        protected override string TableName => $"{AppTablePrefix}UserId";

        public async Task<byte[]> GetUserIdAsync(byte[] userName)
        {
            var entity = await RetrieveEntityUsingPointQueryAsync(
                UserNameAwareEntity.GetPartitionKey(userName),
                UserNameAwareEntity.GetRowKey(userName));

            return entity.UserId;
        }

        public async Task UpsertUserIdAsync(byte[] userName, byte[] userId)
        {
            await UpsertAsync(new UserIdEntity(userId, userName));
        }
    }
}