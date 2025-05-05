using CashFlow.Domain.Repositories.Users;
using Moq;

namespace CommonTesteUtilities.Repositories
{
    public class UserWriteOnlyRepositoryBuilder
    {
        public static IUsersWriteOnlyRepository Build()
        {
            var mock = new Mock<IUsersWriteOnlyRepository>();   
            return mock.Object;
        }
    }
}
