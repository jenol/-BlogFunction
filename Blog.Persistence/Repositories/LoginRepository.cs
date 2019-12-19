using System.Threading.Tasks;
using Blog.Persistence.Entities;
using Microsoft.WindowsAzure.Storage;

namespace Blog.Persistence.Repositories
{
    internal class LoginRepository : RepositoryBase<LoginEntity>, ILoginRepository
    {
        public LoginRepository(string appTablePrefix, CloudStorageAccount cloudStorageAccount) : base(appTablePrefix,
            cloudStorageAccount) { }

        protected override string TableName => $"{AppTablePrefix}Login";

        public async Task<LoginEntity> GetLoginAsync(string loginUserName, string loginPassword)
        {
            if (string.IsNullOrWhiteSpace(loginUserName) || string.IsNullOrWhiteSpace(loginPassword))
            {
                return null;
            }

            var login = await RetrieveEntityUsingPointQueryAsync(UserNameAwareEntity.GetPartitionKey(loginUserName),
                loginUserName);

            return login.Password != loginPassword ? null : login;
        }

        public async Task<LoginEntity> UpsetLoginAsync(string loginUserName, string loginPassword)
        {
            var loginEntry = new LoginEntity(loginUserName, loginPassword);
            return await UpsertAsync(loginEntry);
        }
    }
}