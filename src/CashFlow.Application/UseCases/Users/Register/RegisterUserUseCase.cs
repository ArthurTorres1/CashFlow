using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Users;
using CashFlow.Domain.Security;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using FluentValidation.Results;

namespace CashFlow.Application.UseCases.Users.Register
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersWriteOnlyRepository _repositoryWriteOnly;
        private readonly IUserReadOnlyRepository _repositoryReadOnly;
        private readonly IPasswordEncrypter _passwordEncrypter;

        public RegisterUserUseCase(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUsersWriteOnlyRepository repositoryWriteOnly,
            IUserReadOnlyRepository repositoryReadOnly,
            IPasswordEncrypter passwordEncrypter)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repositoryWriteOnly = repositoryWriteOnly;
            _repositoryReadOnly = repositoryReadOnly;
            _passwordEncrypter = passwordEncrypter;
        }
        public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
        {
            await Validate(request);

            var entity = _mapper.Map<User>(request);
            entity.Password = _passwordEncrypter.Encrypt(request.Password);
            entity.User_identifier = Guid.NewGuid();

            await _repositoryWriteOnly.Add(entity);
            await _unitOfWork.Commit();

            return new ResponseRegisteredUserJson
            {
                Name = entity.Name
            };
        }

        private async Task Validate(RequestRegisterUserJson request)
        {
            var result = new UserValidator().Validate(request);
            
            var emailExist = await _repositoryReadOnly.ExistActiveUserWithEmail(request.Email);
            if (emailExist)
            {
                result.Errors.Add(new ValidationFailure(string.Empty, ResourceErrorMessages.EMAIL_ALREADY_REGISTRED));
            }

            if(result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();
                throw new ErrorOnValidationException(errorMessages);
            }

        }
    }
}
