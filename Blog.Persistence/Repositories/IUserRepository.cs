using System;
using System.Threading.Tasks;
using Blog.Persistence.Entities;

namespace Blog.Persistence.Repositories
{
    public interface IUserRepository
    {
        Task<UserEntity> GetUserAsync(string userName);
        Task<UserEntity> UpsertUserAsync(Guid userId, string userName, string firstName, string lastName, string email);
    }
}