using CashFlow.Application.UseCases.Users;
using CashFlow.Communication.Requests;
using CommonTesteUtilities.Requests;
using FluentAssertions;
using FluentValidation;

namespace Validator.Users
{
    public class PasswordValidatorTest
    {
        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("aa")]
        [InlineData("aaa")]
        [InlineData("aaaa")]
        [InlineData("aaaaa")]
        [InlineData("aaaaaa")]
        [InlineData("aaaaaaa")]
        [InlineData("aaaaaaaa")]
        [InlineData("AAAAAAAA")]
        [InlineData("Aaaaaaaa")]
        [InlineData("Aaaaaaa1")]
        [InlineData(null)]
        public void Error_Email_Empty(string password)
        {
            //Arrange
            var validator = new PasswordValidator<RequestRegisterUserJson>();
            var request = RequestRegisterUserJsonBuilder.Build();

            //Act
            var result = validator.IsValid(new ValidationContext<RequestRegisterUserJson>(new RequestRegisterUserJson()), password);

            //Assert
            result.Should().BeFalse();        }
    }
}
