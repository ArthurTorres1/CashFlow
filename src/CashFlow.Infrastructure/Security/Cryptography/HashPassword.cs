using BCrypt.Net;
using CashFlow.Domain.Security;

namespace CashFlow.Infrastructure.Security.Cryptography
{
    public class HashPassword : IPasswordEncrypter
    {
        public string Encrypt(string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
           
            return passwordHash;
        }

        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
