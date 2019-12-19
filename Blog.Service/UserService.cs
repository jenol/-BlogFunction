using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Blog.Persistence.Entities;
using Blog.Persistence.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using SequentialGuid;

namespace Blog.Service
{
    public class UserService : IUserService
    {
        private const string EncryptionKey = "abc123";
        private readonly ILoginRepository _loginRepository;
        private readonly string _salt = "NZsP6NnmfBuYeJrrAKNuVQ==";
        private readonly string _secret = "somethingyouwantwhichissecurewillworkk";
        private readonly IUserIdRepository _userIdRepository;
        private readonly IUserNameRepository _userNameRepository;
        private readonly IUserRepository _userRepository;

        public UserService(
            ILoginRepository loginRepository,
            IUserRepository userRepository,
            IUserIdRepository userIdRepository,
            IUserNameRepository userNameRepository)
        {
            IdentityModelEventSource.ShowPII = true;
            _loginRepository = loginRepository;
            _userRepository = userRepository;
            _userIdRepository = userIdRepository;
            _userNameRepository = userNameRepository;
        }

        public async Task<IEnumerable<User>> AddUsersAsync(IEnumerable<UserInput> users)
        {
            await ((RepositoryBase) _userRepository).InitTableAsync();
            await ((RepositoryBase) _loginRepository).InitTableAsync();
            await ((RepositoryBase) _userIdRepository).InitTableAsync();
            await ((RepositoryBase) _userNameRepository).InitTableAsync();

            var savedUsers = new List<User>();

            foreach (var user in users)
            {
                var userId = SequentialGuidGenerator.Instance.NewGuid();

                await _userIdRepository.UpsertUserIdAsync(
                    user.UserName,
                    userId);

                await _userRepository.UpsertUserAsync(
                    userId,
                    user.UserName,
                    user.FirstName,
                    user.LastName,
                    EncryptEmail(user.Email));

                await _userNameRepository.UpsertUserIdAsync(user.UserName, userId);

                savedUsers.Add(new User
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user?.FirstName,
                    LastName = user?.LastName
                });

                await _loginRepository.UpsetLoginAsync(user.UserName, HashPassword(user.Password));
            }

            return savedUsers;
        }

        public async Task<User> GetUserBySecurityTokenAsync(string securityToken)
        {
            var login = GetLogin(securityToken);
            return await GetUserAsync(login.UserName);
        }

        public async Task<User> GetUserAsync(string userName)
        {
            var user = await _userRepository.GetUserAsync(userName);
            var id = await _userIdRepository.GetUserIdAsync(userName);

            return new User
            {
                UserId = id,
                UserName = user.UserName,
                Email = DecryptEmail(user.Email),
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

        public async Task<string> GetSecurityTokenAsync(string username, string password)
        {
            var login = await _loginRepository.GetLoginAsync(username, HashPassword(password));

            if (login == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = CreateSecurityTokenDescriptor(username, HashPassword(password));
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private LoginEntity GetLogin(string securityToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters =
                new TokenValidationParameters
                {
                    ValidIssuer = "self",
                    ValidAudiences = new[] {"http://localhost/"},
                    IssuerSigningKeys = new[] {GetSymmetricSecurityKey()}
                };

            var principal = tokenHandler.ValidateToken(securityToken, validationParameters, out _);

            var userName = principal.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;
            var password = principal.Claims.FirstOrDefault(c => c.Type == "Password")?.Value;

            return new LoginEntity(userName, password);
        }

        private SecurityTokenDescriptor CreateSecurityTokenDescriptor(string username, string password)
        {
            return new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserName", username),
                    new Claim("Password", password)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = "self",
                Audience = "http://localhost/",
                SigningCredentials = GetSigningCredentials()
            };
        }

        private SigningCredentials GetSigningCredentials() =>
            new SigningCredentials(
                GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256Signature);

        private SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            var key = Encoding.ASCII.GetBytes(_secret);
            return new SymmetricSecurityKey(key);
        }

        private string HashPassword(string password) => Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            Encoding.UTF8.GetBytes(_salt),
            KeyDerivationPrf.HMACSHA1,
            10000,
            256 / 8));

        private static string EncryptEmail(string email)
        {
            var clearBytes = Encoding.Unicode.GetBytes(email);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(EncryptionKey,
                    new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }

                    email = Convert.ToBase64String(ms.ToArray());
                }
            }

            return email;
        }

        private static string DecryptEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            email = email.Replace(" ", "+");
            var cipherBytes = Convert.FromBase64String(email);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(EncryptionKey,
                    new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }

                    email = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return email;
        }
    }
}