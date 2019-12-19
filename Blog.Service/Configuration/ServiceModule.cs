using Blog.Persistence.Configuration;
using Blog.Persistence.Repositories;
using Guyabano.DepenencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Service.Configuration
{
    public class ServiceModule : IIocModule
    {
        public void AddServices(IFunctionsHostBuilder builder)
        {
            builder.AddModule(new PersistenceModule("pollApp"));
            builder.Services.AddSingleton<IUserService>(p =>
                new UserService(
                    p.GetService<ILoginRepository>(),
                    p.GetService<IUserRepository>(),
                    p.GetService<IUserIdRepository>(),
                    p.GetService<IUserNameRepository>()));
        }
    }
}