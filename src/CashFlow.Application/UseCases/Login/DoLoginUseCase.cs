using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.Users;
using CashFlow.Domain.Security;
using CashFlow.Exception.ExceptionsBase;
using CashFlow.Infrastructure.Security.Tokens;

namespace CashFlow.Application.UseCases.Login
{
    public class DoLoginUseCase : IDoLoginUseCase
    {
        private readonly IUserReadOnlyRepository _repository;
        private readonly IAccessTokenGenerator _tokenGenerator;
        private readonly IPasswordEncrypter _passwordEncrypter;
        public DoLoginUseCase(
            IUserReadOnlyRepository repository,
            IPasswordEncrypter passwordEncrypter,
            IAccessTokenGenerator tokenGenerator
            )
        {
            _repository = repository;
            _passwordEncrypter = passwordEncrypter;
            _tokenGenerator = tokenGenerator;
        }
        public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
        {
            Validate(request);

            var user = await _repository.GetUserByEmail(request.Email);
            if (user is null)
                throw new InvalidLoginException();

            var passwordMatch = _passwordEncrypter.Verify(request.Password, user.Password);
            if (passwordMatch == false)
                throw new InvalidLoginException();

            return new ResponseRegisteredUserJson
            {
                Name = user.Name,
                Token = _tokenGenerator.Generate(user)
            };
        }

        private void Validate(RequestLoginJson request)
        {
            var result = new LoginValidator().Validate(request);
            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();
                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
