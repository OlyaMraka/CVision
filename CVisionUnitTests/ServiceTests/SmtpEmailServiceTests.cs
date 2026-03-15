using CVision.BLL.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CVisionUnitTests.ServiceTests;

public class SmtpEmailServiceTests
{
    private readonly Mock<IConfiguration> _configMock;

    public SmtpEmailServiceTests()
    {
        _configMock = new Mock<IConfiguration>();
    }

    [Fact]
    public async Task SendEmailAsync_ShouldThrowException_WhenSettingsAreMissing()
    {
        // Arrange
        _configMock.Setup(x => x["EmailSettings:Sender"]).Returns(string.Empty);
        _configMock.Setup(x => x["EmailSettings:Password"]).Returns(string.Empty);

        var service = new SmtpEmailService(_configMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendEmailAsync("test@user.com", "Subject", "Message"));

        Assert.Equal("Email settings are missing in configuration.", exception.Message);
    }

    [Fact]
    public async Task SendConfirmationEmailAsync_ShouldCallInternalSendLogic()
    {
        // Arrange
        _configMock.Setup(x => x["EmailSettings:Sender"]).Returns("admin@cvision.com");
        _configMock.Setup(x => x["EmailSettings:Password"]).Returns("secure_password");

        var service = new SmtpEmailService(_configMock.Object);
        var testEmail = "customer@gmail.com";
        var link = "https://cvision.com/confirm?token=123";

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() =>
            service.SendConfirmationEmailAsync(testEmail, link));

        Assert.Contains("Помилка при відправці пошти", ex.Message);
    }
}