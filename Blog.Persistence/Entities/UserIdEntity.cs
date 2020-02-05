namespace Blog.Persistence.Entities
{
    public class UserIdEntity : UserNameAwareEntity
    {
        public UserIdEntity() { }

        public UserIdEntity(byte[] userId, byte[] userName)
        {
            UserName = userName;
            UserId = userId;
        }

        public byte[] UserId { get; set; }
    }
}