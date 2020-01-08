namespace Blog.Persistence.Entities
{
    public class LoginEntity : UserNameAwareEntity
    {
        public LoginEntity() { }

        public LoginEntity(string userName, byte[] password)
        {
            UserName = userName;
            Password = password;
        }

        public byte[] Password { get; set; }
    }
}