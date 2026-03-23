using CVision.BLL.Commands.Users.UpdateProfile;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Users;
using CVision.BLL.Validators.Users;
using FluentValidation.TestHelper;

namespace CVisionUnitTests.ValidatorsTests;

public class UpdateProfileValidatorTests
{
    private readonly UpdateProfileValidator _validator;

    public UpdateProfileValidatorTests()
    {
        _validator = new UpdateProfileValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        // Arrange
        var command = new UpdateProfileCommand(1, new UpdateProfileRequestDto { Email = string.Empty, UserName = "ValidUser" });

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Email)
            .WithErrorMessage(UserConstants.EmailRequiredErrorMessage);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Too_Long()
    {
        // Arrange
        var longEmail = new string('a', UserConstants.MaxEmailLength + 1) + "@test.com";
        var command = new UpdateProfileCommand(1, new UpdateProfileRequestDto { Email = longEmail, UserName = "ValidUser" });

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Email)
            .WithErrorMessage(UserConstants.MaxEmailLengthErrorMessage);
    }

    [Fact]
    public void Should_Have_Error_When_UserName_Is_Empty()
    {
        // Arrange
        var command = new UpdateProfileCommand(1, new UpdateProfileRequestDto { Email = "test@test.com", UserName = string.Empty });

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RequestDto.UserName)
            .WithErrorMessage(UserConstants.UserNameRequiredErrorMessage);
    }

    [Fact]
    public void Should_Have_Error_When_UserName_Is_Too_Short()
    {
        // Arrange
        var command = new UpdateProfileCommand(1, new UpdateProfileRequestDto { Email = "test@test.com", UserName = "ab" });

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RequestDto.UserName)
            .WithErrorMessage(UserConstants.MinUserNameErrorMessage);
    }

    [Fact]
    public void Should_Have_Error_When_UserName_Is_Too_Long()
    {
        // Arrange
        var longName = new string('a', UserConstants.MaxUserNameLength + 1);
        var command = new UpdateProfileCommand(1, new UpdateProfileRequestDto { Email = "test@test.com", UserName = longName });

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RequestDto.UserName)
            .WithErrorMessage(UserConstants.MaxUserNameErrorMessage);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        // Arrange
        var command = new UpdateProfileCommand(1, new UpdateProfileRequestDto
        {
            Email = "test@example.com",
            UserName = "ValidUser",
            PhoneNumber = "123456789",
        });

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
