using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Blog.Persistence.Entities;
using Blog.Persistence.Repositories;
using Blog.Service.DomainObjects;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Service
{
    internal class AuthenticationService : ServiceBase, IAuthenticationService
    {
        private readonly ILoginRepository _loginRepository;

        public AuthenticationService(
            string encryptionKey,
            string salt,
            ILoginRepository loginRepository) : base(encryptionKey, salt)
        {
            IdentityModelEventSource.ShowPII = true;
            _loginRepository = loginRepository;
        }

        public Task<LoginDetails> GetLoginAsync(string securityToken)
        {
            var enitity = GetLogin(securityToken);

            return Task.FromResult(new LoginDetails
            {
                UserName = GetDecryptedText(enitity.UserName)
            });
        }

        public async Task<LoginDetails> GetLoginAsync(string username, string password)
        {
            var enitity = await _loginRepository.GetLoginAsync(GetEncryptedBytes(username), HashPassword(password));

            return new LoginDetails
            {
                UserName = GetDecryptedText(enitity.UserName)
            };
        }

        public Task UpsetLoginAsync(string username, string password) =>
            _loginRepository.UpsetLoginAsync(GetEncryptedBytes(username), HashPassword(password));


        public async Task<string> GetSecurityTokenAsync(string username, string password)
        {
            var login = await _loginRepository.GetLoginAsync(GetEncryptedBytes(username), HashPassword(password));

            if (login == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = CreateSecurityTokenDescriptor(username, Encoding.UTF8.GetBytes(password));
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

            return new LoginEntity(GetEncryptedBytes(userName), HashPassword(password));
        }

        private SecurityTokenDescriptor CreateSecurityTokenDescriptor(string username, byte[] password)
        {
            return new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserName", username),
                    new Claim("Password", Encoding.UTF8.GetString(password))
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
            var key = Encoding.ASCII.GetBytes(EncryptionKey);
            return new SymmetricSecurityKey(key);
        }

        private byte[] HashPassword(string password) => KeyDerivation.Pbkdf2(
            password,
            Salt,
            KeyDerivationPrf.HMACSHA1,
            10000,
            256 / 8);
    }
}