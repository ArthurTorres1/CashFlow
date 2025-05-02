using CashFlow.Communication.Requests;
using CashFlow.Exception;
using FluentValidation;

namespace CashFlow.Application.UseCases.Login
{
    public class LoginValidator : AbstractValidator<RequestLoginJson>
    {
        public LoginValidator()
        {
            RuleFor(email => email.Email)
                .NotEmpty()
                .WithMessage(ResourceErrorMessages.EMAIL_EMPTY)
                .EmailAddress()
                .WithMessage(ResourceErrorMessages.EMAIL_INVALID);
        }
    }
}
