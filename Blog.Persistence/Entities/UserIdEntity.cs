using System;

namespace Blog.Persistence.Entities
{
    public class UserIdEntity : UserNameAwareEntity
    {
        public UserIdEntity() { }

        public UserIdEntity(Guid userId, string userName)
        {
            UserName = userName;
            UserId = userId;
        }

        public Guid UserId { get; set; }
    }
}