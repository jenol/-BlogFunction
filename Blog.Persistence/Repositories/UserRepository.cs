using System.Threading.Tasks;
using Blog.Persistence.Entities;
using Microsoft.WindowsAzure.Storage;

namespace Blog.Persistence.Repositories
{
    internal class UserRepository : RepositoryBase<UserEntity>, IUserRepository
    {
        public UserRepository(string appTablePrefix, CloudStorageAccount cloudStorageAccount) : base(appTablePrefix,
            cloudStorageAccount) { }

        protected override string TableName => $"{AppTablePrefix}User";

        public async Task<UserEntity> GetUserAsync(byte[] userName) =>
            await RetrieveEntityUsingPointQueryAsync(UserNameAwareEntity.GetPartitionKey(userName),
                UserNameAwareEntity.GetRowKey(userName));

        public async Task<UserEntity> UpsertUserAsync(byte[] userId, byte[] userName, string firstName, string lastName,
            string email)
        {
            var user = await GetUserAsync(userName) ?? new UserEntity {UserName = userName};
            user.UserId = userId;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;

            return await UpsertAsync(user);
        }
    }
}