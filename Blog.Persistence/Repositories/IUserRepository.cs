using System;
using System.Threading.Tasks;
using Blog.Persistence.Entities;

namespace Blog.Persistence.Repositories
{
    public interface IUserRepository
    {
        Task<UserEntity> GetUserAsync(byte[] userName);
        Task<UserEntity> UpsertUserAsync(Guid userId, byte[] userName, string firstName, string lastName, string email);
    }
}