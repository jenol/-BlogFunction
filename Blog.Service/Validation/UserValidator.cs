using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blog.Service.Contracts;
using Blog.Service.DomainObjects;

namespace Blog.Service.Validation
{
    public class UserValidator
    {
        private readonly IEmailService _emailService;

        public UserValidator(IEmailService emailService) => _emailService = emailService;

        public async Task<IEnumerable<(int ImportOperationId, string Message)>> ValidateAsync(UserImport[] users)
        {
            if (users == null || !users.Any())
            {
                throw new UserValidatorException("The emails parameter must be not null or empty");
            }

            var usersByImportId = users.GroupBy(u => u.ImportOperationId).ToDictionary(k => k.Key, g => g.ToArray());

            if (usersByImportId.Any(g => g.Value.Length > 1))
            {
                throw new UserValidatorException("All imported users must have a unique ImportOperationId");
            }

            var errors = Validate(users).ToList();

            var usersByEmail = await _emailService.GetUserNamesByEmailsAsync(users.Select(u => u.Email).Distinct());

            foreach (var userWithAlreadyUsedEmails in users.Where(u => usersByEmail.ContainsKey(u.Email)))
            {
                errors.Add((userWithAlreadyUsedEmails.ImportOperationId, "email is already is in use"));
            }

            return errors;
        }

        private IEnumerable<(int ImportOperationId, string Message)> Validate(UserImport[] users)
        {
            var validEmails = new HashSet<string>();

            foreach (var user in users)
            {
                if (!RegexUtilities.IsValidEmail(user.Email))
                {
                    yield return (user.ImportOperationId, "contains invalid email");
                }
                else
                {
                    if (validEmails.Contains(user.Email))
                    {
                        yield return (user.ImportOperationId, "contains duplicated email");
                    }
                    else
                    {
                        validEmails.Add(user.Email);
                    }
                }

                if (!ValidatePassword(user.Password, out var passwordScore))
                {
                    yield return (user.ImportOperationId, $"contains invalid password. The password score is {passwordScore}");
                }

                if (string.IsNullOrWhiteSpace(user.FirstName))
                {
                    yield return (user.ImportOperationId, "contains invalid first name");
                }

                if (string.IsNullOrWhiteSpace(user.LastName))
                {
                    yield return (user.ImportOperationId, "contains invalid last name");
                }
            }
        }

        private bool ValidatePassword(string password, out PasswordScore passwordScore)
        {
            passwordScore = CheckStrength(password);

            switch (passwordScore) {
                case PasswordScore.Blank:
                case PasswordScore.VeryWeak:
                case PasswordScore.Weak:
                case PasswordScore.Medium:
                    return false;
                default:
                    return true;
            }
        }

        private static PasswordScore CheckStrength(string password)
        {
            var chars = "!@#$%^&*?_~-£()";

            if (string.IsNullOrWhiteSpace(password))
            {
                return PasswordScore.Blank;
            }

            var score = 0;

            if (password.Length >= 8)
            {
                score++;
            }
            else
            {
                return PasswordScore.VeryWeak;
            }

            if (password.Length >= 12)
            {
                score++;
            }

            if (password.Any(char.IsDigit))
            {
                score++;
            }

            if (password.Any(char.IsLetter))
            {
                score++;
            }

            if (password.Any(c => chars.Contains(c)))
            {
                score++;
            }

            return (PasswordScore) score;
        }
    }
}