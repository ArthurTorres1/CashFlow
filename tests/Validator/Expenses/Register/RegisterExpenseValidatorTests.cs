using CashFlow.Application.UseCases.Expenses;
using CashFlow.Communication.Enums;
using CashFlow.Exception;
using CommonTesteUtilities.Requests;
using FluentAssertions;

namespace Validator.Expenses.Register
{
    public class RegisterExpenseValidatorTests
    {
        [Fact]
        public void Success()
        {
            //Arrange
            var validator = new ExpenseValidator();
            var request = RequestRegisterExpenseJsonBuilder.Build();
            //Act
            var resultValidator = validator.Validate(request);
            //Assert
            resultValidator.IsValid.Should().BeTrue();

        }

        //Usando o Theory para passar parametro e testar mais de um valor 
        [Theory]
        [InlineData("")]
        [InlineData("      ")]
        [InlineData(null)]
        public void Error_Title_Empty(string title)
        {
            //Arrange
            var validator = new ExpenseValidator();
            var request = RequestRegisterExpenseJsonBuilder.Build();
            request.Title = title;
            //Act
            var resultValidator = validator.Validate(request);
            //Assert qual resposta eu espero do meu teste neste caso um falso - utilizando o pacote FluentAssertions
            resultValidator.IsValid.Should().BeFalse();
            resultValidator.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.TITLE_REQUIRED));
        }

        [Fact]
        public void Error_Date_Future()
        {
            //Arrange
            var validator = new ExpenseValidator();
            var request = RequestRegisterExpenseJsonBuilder.Build();
            request.Date = DateTime.UtcNow.AddDays(1);
            //Act
            var resultValidator = validator.Validate(request);
            //Assert qual resposta eu espero do meu teste neste caso um falso - utilizando o pacote FluentAssertions
            resultValidator.IsValid.Should().BeFalse();
            resultValidator.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.EXPENSES_CANNOT_FOR_THE_FUTURE));
        }

        [Fact]
        public void Error_Payment_Type_Invalid()
        {
            //Arrange
            var validator = new ExpenseValidator();
            var request = RequestRegisterExpenseJsonBuilder.Build();
            request.PaymentType = (PaymentType)700;
            //Act
            var resultValidator = validator.Validate(request);
            //Assert qual resposta eu espero do meu teste neste caso um falso - utilizando o pacote FluentAssertions
            resultValidator.IsValid.Should().BeFalse();
            resultValidator.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.PAYMENT_TYPE_INVALID));
        }

        //Usando o Theory para passar parametro e testar mais valores no amount
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-2)]
        public void Error_Amount_Invalid(decimal amount)
        {
            //Arrange
            var validator = new ExpenseValidator();
            var request = RequestRegisterExpenseJsonBuilder.Build();
            request.Amount = amount;
            //Act
            var resultValidator = validator.Validate(request);
            //Assert qual resposta eu espero do meu teste neste caso um falso - utilizando o pacote FluentAssertions
            resultValidator.IsValid.Should().BeFalse();
            resultValidator.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.AMOUNT_MUST__BE_GREATER_THAN_ZERO));
        }


    }
}
