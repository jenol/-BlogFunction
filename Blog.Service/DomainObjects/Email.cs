using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Blog.Service.Tests")]

namespace Blog.Service.DomainObjects
{
    public sealed class Email
    {
        private readonly string _emailText;

        internal Email(string emailText) => _emailText = emailText;

        public static bool TryParse(string emailText, out Email email)
        {
            if (RegexUtilities.IsValidEmail(emailText))
            {
                email = new Email(emailText);
                return true;
            }

            email = null;
            return false;
        }

        public override string ToString() => _emailText;
    }
}