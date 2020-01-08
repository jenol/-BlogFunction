using System.Threading.Tasks;

namespace Blog.Persistence.Repositories
{
    public interface IDbSetup
    {
        Task Run();
    }
}