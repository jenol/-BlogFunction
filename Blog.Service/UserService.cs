using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Persistence.Repositories;
using Blog.Service.Contracts;
using Blog.Service.DomainObjects;
using Blog.Service.Validation;
using SequentialGuid;

namespace Blog.Service
{
    public class UserService : ServiceBase, IUserService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IDbSetup _dbSetup;
        private readonly IEmailService _emailService;
        private readonly IUserIdRepository _userIdRepository;
        private readonly IUserNameRepository _userNameRepository;
        private readonly IUserRepository _userRepository;

        public UserService(
            string encryptionKey, 
            string salt,
            IDbSetup dbSetup,
            IUserRepository userRepository,
            IUserIdRepository userIdRepository,
            IUserNameRepository userNameRepository,
            IEmailService emailService,
            IAuthenticationService authenticationService) : base(encryptionKey, salt)
        {
            _dbSetup = dbSetup;
            _userRepository = userRepository;
            _userIdRepository = userIdRepository;
            _userNameRepository = userNameRepository;
            _emailService = emailService;
            _authenticationService = authenticationService;
        }

        public async Task<IEnumerable<UserImportOperationOutcome>> AddUsersAsync(UserImport[] users)
        {
            await _dbSetup.Run();

            var savedUsers = new List<UserImportOperationOutcome>();

            var o = new UserValidator(_emailService);

            var validationResult = (await o.ValidateAsync(users)).ToArray();

            if (validationResult.Any())
            {
                var resultsById = validationResult
                    .GroupBy(v => v.ImportOperationId).ToDictionary(t => t.Key, t => t.Select(m => m.Message) .ToArray());

                foreach (var result in resultsById)
                {
                    savedUsers.Add(new UserImportOperationOutcome
                    {
                        ImportOperationId = result.Key,
                        IsSuccess = false,
                        Notes = result.Value
                    });
                }

                return savedUsers;
            }

            foreach (var user in users)
            {
                if (!Email.TryParse(user.Email, out var email))
                {
                    continue;
                }

                var encryptedUserName = GetEncryptedBytes(user.UserName);

                await _emailService.UpsertEmailAsync(encryptedUserName, email);

                var user1 = await _userRepository.GetUserAsync(encryptedUserName);

                byte[] userId = null;

                if (user1 != null)
                {
                    userId = await _userIdRepository.GetUserIdAsync(encryptedUserName);
                }

                if (userId == null || !userId.Any())
                {
                    userId = SequentialGuidGenerator.Instance.NewGuid().ToByteArray();
                    await _userIdRepository.UpsertUserIdAsync(encryptedUserName, userId);
                }

                await _userNameRepository.UpsertUserIdAsync(encryptedUserName, userId);

                await _userRepository.UpsertUserAsync(
                    userId,
                    encryptedUserName,
                    GetEncryptedText(user.FirstName),
                    GetEncryptedText(user.LastName),
                    _emailService.EncryptEmail(email).ToString());

                await _authenticationService.UpsetLoginAsync(user.UserName, user.Password);

                savedUsers.Add(new UserImportOperationOutcome
                {
                    ImportOperationId = user.ImportOperationId,
                    UserId = new Guid(userId),
                    IsSuccess = true,
                    Notes = new string[0]
                });
            }

            return savedUsers;
        }

        public Task<User> GetUserAsync(string userName) => GetUserAsync(userName, GetEncryptedBytes(userName));

        private async Task<User> GetUserAsync(string userName, byte[] encryptedUserName)
        {
            
            var user = await _userRepository.GetUserAsync(encryptedUserName);

            if (user == null)
            {
                return null;
            }

            if (!user.UserId.Any())
            {
                user.UserId = await _userIdRepository.GetUserIdAsync(encryptedUserName);
            }

            var email = user.Email == null ? null : _emailService.DecryptEmail(new EncryptedEmail(user?.Email)).ToString();

            return new User
            {
                UserId = Guid.Parse(Encoding.UTF8.GetString(user.UserId)),
                UserName = userName,
                Email = email,
                FirstName = GetDecryptedText(user.FirstName),
                LastName = GetDecryptedText(user.LastName)
            };
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            var encryptedUserName = await _userNameRepository.GetUserNameAsync(userId.ToByteArray());

            if (!encryptedUserName.Any())
            {
                return null;
            }

            return await GetUserAsync(GetDecryptedText(encryptedUserName), encryptedUserName);
        }
    }
}