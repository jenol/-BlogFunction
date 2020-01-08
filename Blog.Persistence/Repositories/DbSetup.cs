using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Persistence.Repositories
{
    internal class DbSetup : IDbSetup
    {
        private readonly IEnumerable<RepositoryBase> _repositories;

        public DbSetup(IEnumerable<RepositoryBase> repositories) => _repositories = repositories;

        public async Task Run()
        {
            await Task.WhenAll(_repositories.Select(r => r.InitTableAsync()));
        }
    }
}