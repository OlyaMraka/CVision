using CVision.BLL.Validators.Users;
using CVision.BLL.DTOs.Users;
using CVision.BLL.Constans;
using FluentValidation.TestHelper;

namespace CVisionUnitTests.ValidatorsTests;

public class BaseUserValidatorTests
{
    private readonly BaseUserValidator _validator;

    public BaseUserValidatorTests()
    {
        _validator = new BaseUserValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        // Arrange
        var model = new RegisterUserRequestDto { Email = string.Empty };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage(UserConstants.EmailRequiredErrorMessage);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Too_Long()
    {
        // Arrange
        var longEmail = new string('a', UserConstants.MaxEmailLength + 1) + "@test.com";
        var model = new RegisterUserRequestDto { Email = longEmail };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage(UserConstants.MaxEmailLengthErrorMessage);
    }

    [Fact]
    public void Should_Have_Error_When_UserName_Is_Too_Short()
    {
        // Arrange
        var model = new RegisterUserRequestDto { UserName = "ab" };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.UserName)
            .WithErrorMessage(UserConstants.MinUserNameErrorMessage);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        // Arrange
        var model = new RegisterUserRequestDto
        {
            Email = "test@example.com",
            UserName = "ValidUser123",
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}