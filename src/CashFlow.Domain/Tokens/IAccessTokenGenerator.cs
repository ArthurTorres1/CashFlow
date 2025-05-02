using CashFlow.Domain.Entities;

namespace CashFlow.Infrastructure.Security.Tokens
{
    public interface IAccessTokenGenerator
    {
        string Generate(User user);
    }
}
