using System.Linq;
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

        public async Task<LoginEntity> GetLoginAsync(byte[] userName, byte[] password)
        {
            if (!userName.Any() || !password.Any())
            {
                return null;
            }

            var login = await RetrieveEntityUsingPointQueryAsync(
                UserNameAwareEntity.GetPartitionKey(userName), 
                UserNameAwareEntity.GetRowKey(userName));

            return login.Password != password ? null : login;
        }

        public async Task<LoginEntity> UpsetLoginAsync(byte[] userName, byte[] password)
        {
            var loginEntry = new LoginEntity(userName, password);
            return await UpsertAsync(loginEntry);
        }
    }
}