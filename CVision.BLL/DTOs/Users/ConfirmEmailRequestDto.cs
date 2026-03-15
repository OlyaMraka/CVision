namespace CVision.BLL.DTOs.Users;

public class ConfirmEmailRequestDto
{
    public int UserId { get; set; }

    public string Token { get; set; } = string.Empty;
}