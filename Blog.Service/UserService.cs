using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Persistence.Repositories;
using Blog.Service.DomainObjects;
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

        public async Task<IEnumerable<User>> AddUsersAsync(IEnumerable<UserInput> users)
        {
            await _dbSetup.Run();

            var emailsAdded = new HashSet<Email>();
            var idsAdded = new HashSet<Guid>();

            var savedUsers = new List<User>();

            var o = await _emailService.GetUserNamesByEmailsAsync(users.Select(u => u.Email));

            foreach (var user in users)
            {
                if (!Email.TryParse(user.Email, out var email))
                {
                    continue;
                }

                if (emailsAdded.Contains(email))
                {
                    continue;
                }

                emailsAdded.Add(email);

                if (o.ContainsKey(email.ToString()))
                {
                    continue;
                }

                var encryptedUserName = GetEncryptedBytes(user.UserName);

                await _emailService.UpsertEmailAsync(encryptedUserName, email);

                var user1 = await _userRepository.GetUserAsync(encryptedUserName);

                Guid? userId = null;

                if (user1 != null)
                {
                    userId = await _userIdRepository.GetUserIdAsync(encryptedUserName);
                }

                if (!userId.HasValue)
                {
                    userId = SequentialGuidGenerator.Instance.NewGuid();
                    await _userIdRepository.UpsertUserIdAsync(encryptedUserName, userId.Value);
                }

                await _userNameRepository.UpsertUserIdAsync(encryptedUserName, userId.Value);

                if (idsAdded.Contains(userId.Value))
                {
                    continue;
                }

                idsAdded.Add(userId.Value);

                await _userRepository.UpsertUserAsync(
                    userId.Value,
                    encryptedUserName,
                    GetEncryptedText(user.FirstName),
                    GetEncryptedText(user.LastName),
                    _emailService.EncryptEmail(email).ToString());

                await _authenticationService.UpsetLoginAsync(user.UserName, user.Password);

                savedUsers.Add(new User
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user?.FirstName,
                    LastName = user?.LastName
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

            if (user.UserId == Guid.Empty)
            {
                user.UserId = await _userIdRepository.GetUserIdAsync(encryptedUserName) ?? Guid.Empty;
            }

            var email = user.Email == null ? null : _emailService.DecryptEmail(new EncryptedEmail(user?.Email)).ToString();

            return new User
            {
                UserId = user.UserId,
                UserName = userName,
                Email = email,
                FirstName = GetDecryptedText(user.FirstName),
                LastName = GetDecryptedText(user.LastName)
            };
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            var encryptedUserName = await _userNameRepository.GetUserNameAsync(userId);

            if (!encryptedUserName.Any())
            {
                return null;
            }

            return await GetUserAsync(GetDecryptedText(encryptedUserName), encryptedUserName);
        }
    }
}