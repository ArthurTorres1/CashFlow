using CashFlow.Domain.Security;

namespace CashFlow.Infrastructure.Security
{
    public class HashPassword : IPasswordEncrypter
    {
        public string Encrypt(string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            return passwordHash;
        }
    }
}
