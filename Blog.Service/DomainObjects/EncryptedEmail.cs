namespace Blog.Service.DomainObjects
{
    public sealed class EncryptedEmail
    {
        private readonly string _emailText;

        internal EncryptedEmail(string emailText) => _emailText = emailText;

        public override string ToString() => _emailText;
    }
}