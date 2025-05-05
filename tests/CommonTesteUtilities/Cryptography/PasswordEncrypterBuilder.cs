using CashFlow.Domain.Security;
using Moq;

namespace CommonTesteUtilities.Cryptography
{
    public class PasswordEncrypterBuilder
    {
        public static IPasswordEncrypter Build()
        {
            var mock = new Mock<IPasswordEncrypter>();
            mock.Setup(passwordEncrypter => passwordEncrypter.Encrypt(It.IsAny<string>())).Returns("!#re41@ha9(");

            return mock.Object;
        }
    }
}
