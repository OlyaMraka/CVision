namespace CVision.BLL.DTOs.Users;

public class LoginUserResponseDto
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
