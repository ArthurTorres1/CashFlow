using CashFlow.Application.UseCases.Users;
using CashFlow.Exception;
using CommonTesteUtilities.Requests;
using FluentAssertions;

namespace Validator.Users.Register
{
    public class RegisterUserValidatorTest
    {
        [Fact]
        public void Succes()
        {
            //Arrange
            var validator = new UserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();

            //Act
            var result = validator.Validate(request);

            //Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData(null)]
        public void Error_Name_Empty(string name)
        {
            //Arrange
            var validator = new UserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = name;

            //Act
            var result = validator.Validate(request);

            //Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.NAME_EMPTY));
        }

        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData(null)]
        public void Error_Email_Empty(string email)
        {
            //Arrange
            var validator = new UserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Email = email;

            //Act
            var result = validator.Validate(request);

            //Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.EMAIL_EMPTY));
        }

        [Fact]
        public void Error_Email_Invalid()
        {
            //Arrange
            var validator = new UserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Email = "arthur.com";

            //Act
            var result = validator.Validate(request);

            //Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.EMAIL_INVALID));
        }

        [Fact]
        public void Password_Empty()
        {
            //Arrange
            var validator = new UserValidator();
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Password = "";

            //Act
            var result = validator.Validate(request);

            //Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.INVALID_PASSWORD));
        }
    }
}
