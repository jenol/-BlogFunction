using System;
using System.Threading.Tasks;
using Blog.Persistence.Entities;
using Microsoft.WindowsAzure.Storage;

namespace Blog.Persistence.Repositories
{
    public class UserNameRepository : RepositoryBase<UserNameEntity>, IUserNameRepository
    {
        public UserNameRepository(string appTablePrefix, CloudStorageAccount cloudStorageAccount) :
            base(appTablePrefix, cloudStorageAccount) { }

        protected override string TableName => $"{AppTablePrefix}UserName";

        public async Task<string> GetUserNameAsync(Guid userId) =>
            (await RetrieveEntityUsingPointQueryAsync(UserNameEntity.GetPartitionKey(userId), userId.ToString()))
            .UserName;

        public async Task UpsertUserIdAsync(string userName, Guid userId)
        {
            await UpsertAsync(new UserNameEntity(userId, userName));
        }
    }
}