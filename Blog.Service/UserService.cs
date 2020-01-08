using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Persistence.Repositories;
using SequentialGuid;

namespace Blog.Service
{
    public class UserService : IUserService
    {
        private readonly IDbSetup _dbSetup;
        private readonly IUserIdRepository _userIdRepository;
        private readonly IUserNameRepository _userNameRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IAuthenticationService _authenticationService;

        public UserService(
            IDbSetup dbSetup,
            IUserRepository userRepository,
            IUserIdRepository userIdRepository,
            IUserNameRepository userNameRepository,
            IEmailService emailService,
            IAuthenticationService authenticationService)
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

            var emailsAdded = new HashSet<string>();
            var idsAdded = new HashSet<Guid>();

            var savedUsers = new List<User>();

            var o = await _emailService.GetUserNamesByEmailsAsync(users.Select(u => u.Email));

            foreach (var user in users)
            {
                if (emailsAdded.Contains(user.Email))
                {
                    continue;
                }

                emailsAdded.Add(user.Email);

                var email = user.Email;

                if (o.ContainsKey(email))
                {
                    continue;
                }

                await _emailService.UpsertEmailAsync(user.UserName, email);

                var user1 = await _userRepository.GetUserAsync(user.UserName);

                Guid userId;

                if (user1 != null)
                {
                    userId = await _userIdRepository.GetUserIdAsync(user.UserName);
                }
                else
                {
                    userId = SequentialGuidGenerator.Instance.NewGuid();
                    await _userIdRepository.UpsertUserIdAsync(user.UserName, userId);
                    await _userNameRepository.UpsertUserIdAsync(user.UserName, userId);
                }

                if (idsAdded.Contains(userId))
                {
                    continue;
                }

                idsAdded.Add(userId);

                await _userRepository.UpsertUserAsync(
                    userId,
                    user.UserName,
                    user.FirstName,
                    user.LastName,
                    email);

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

        public async Task<User> GetUserAsync(string userName)
        {
            var user = await _userRepository.GetUserAsync(userName);

            if (user == null)
            {
                return null;
            }

            if (user.UserId == Guid.Empty)
            {
                user.UserId = await _userIdRepository.GetUserIdAsync(userName);
            }

            return new User
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user?.Email,
                FirstName = user?.FirstName,
                LastName = user?.LastName
            };
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            var userName = await _userNameRepository.GetUserNameAsync(userId);

            if (string.IsNullOrWhiteSpace(userName))
            {
                return null;
            }

            return await GetUserAsync(userName);
        }
    }
}