namespace CVision.BLL.DTOs.Users;

public class RegisterUserResponseDto
{
    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool EmailConfirmed { get; set; }
}