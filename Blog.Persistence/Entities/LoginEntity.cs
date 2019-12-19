namespace Blog.Persistence.Entities
{
    public class LoginEntity : UserNameAwareEntity
    {
        public LoginEntity() { }

        public LoginEntity(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public string Password { get; set; }
    }
}