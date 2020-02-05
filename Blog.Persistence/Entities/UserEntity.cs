namespace Blog.Persistence.Entities
{
    public class UserEntity : UserNameAwareEntity
    {
        public UserEntity() { }

        public UserEntity(byte[] userId, byte[] userName, string firstName, string lastName, string email)
        {
            UserId = userId;
            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }

        public byte[] UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}