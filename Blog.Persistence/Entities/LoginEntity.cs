namespace Blog.Persistence.Entities
{
    public class LoginEntity : UserNameAwareEntity
    {
        public LoginEntity() { }

        public LoginEntity(byte[] userName, byte[] password)
        {
            UserName = userName;
            Password = password;
        }

        public byte[] Password { get; set; }
    }
}