using System;
using System.Threading.Tasks;
using Blog.Persistence.Entities;
using Microsoft.WindowsAzure.Storage;

namespace Blog.Persistence.Repositories
{
    public class UserIdRepository : RepositoryBase<UserIdEntity>, IUserIdRepository
    {
        public UserIdRepository(string appTablePrefix, CloudStorageAccount cloudStorageAccount) :
            base(appTablePrefix, cloudStorageAccount) { }

        protected override string TableName => $"{AppTablePrefix}UserId";

        public async Task<Guid> GetUserIdAsync(string userName) =>
            (await RetrieveEntityUsingPointQueryAsync(UserNameAwareEntity.GetPartitionKey(userName), userName)).UserId;

        public async Task UpsertUserIdAsync(string userName, Guid userId)
        {
            await UpsertAsync(new UserIdEntity(userId, userName));
        }
    }
}