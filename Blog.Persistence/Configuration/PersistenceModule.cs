using Blog.Persistence.Repositories;
using Guyabano.DepenencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;

namespace Blog.Persistence.Configuration
{
    public class PersistenceModule : IIocModule
    {
        private readonly string _appTablePrefix;

        public PersistenceModule(string appTablePrefix) => _appTablePrefix = appTablePrefix;

        public void AddServices(IFunctionsHostBuilder builder)
        {
            //var storageQueueConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            //var storageAccount = CloudStorageAccount.Parse(storageQueueConnectionString);

            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            builder.Services.AddSingleton<IUserRepository>(p =>
                new UserRepository(_appTablePrefix, storageAccount));
            builder.Services.AddSingleton<ILoginRepository>(p =>
                new LoginRepository(_appTablePrefix, storageAccount));
            builder.Services.AddSingleton<IUserIdRepository>(p =>
                new UserIdRepository(_appTablePrefix, storageAccount));
            builder.Services.AddSingleton<IUserNameRepository>(p =>
                new UserNameRepository(_appTablePrefix, storageAccount));
            builder.Services.AddSingleton<IEmailRepository>(p =>
                new EmailRepository(_appTablePrefix, storageAccount));

            builder.Services.AddSingleton<IDbSetup>(p => new DbSetup(new[]
            {
                (RepositoryBase) p.GetService<IUserRepository>(),
                (RepositoryBase) p.GetService<ILoginRepository>(),
                (RepositoryBase) p.GetService<IUserIdRepository>(),
                (RepositoryBase) p.GetService<IUserNameRepository>(),
                (RepositoryBase) p.GetService<IEmailRepository>()
            }));
        }
    }
}