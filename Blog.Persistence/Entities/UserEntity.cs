using System;

namespace Blog.Persistence.Entities
{
    public class UserEntity : UserNameAwareEntity
    {
        public UserEntity() { }

        public UserEntity(Guid userId, string userName, string firstName, string lastName, string email)
        {
            UserId = userId;
            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }

        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}