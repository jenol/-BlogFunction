using System.Threading.Tasks;
using Blog.Service.DomainObjects;

namespace Blog.Service
{
    public interface IAuthenticationService
    {
        Task<LoginDetails> GetLoginAsync(string securityToken);
        Task<string> GetSecurityTokenAsync(string username, string password);
        Task<LoginDetails> GetLoginAsync(string username, string password);
        Task UpsetLoginAsync(string username, string password);
    }
}