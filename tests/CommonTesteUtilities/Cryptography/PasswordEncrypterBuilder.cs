using CashFlow.Domain.Security;
using Moq;

namespace CommonTesteUtilities.Cryptography
{
    public class PasswordEncrypterBuilder
    {
        private readonly Mock<IPasswordEncrypter> _mock;

        public PasswordEncrypterBuilder()
        {
            _mock = new Mock<IPasswordEncrypter>();
            _mock.Setup(passwordEncrypter => passwordEncrypter.Encrypt(It.IsAny<string>())).Returns("!Aa%akl26.sJoq9");
        }

        public PasswordEncrypterBuilder Verify(string? password)
        {
            if(string.IsNullOrWhiteSpace(password) == false)
            {
                _mock.Setup(passworEncrypter => passworEncrypter.Verify(password, It.IsAny<string>())).Returns(true);
            }

            return this;
        }
        public IPasswordEncrypter Build() => _mock.Object;
    }
}
