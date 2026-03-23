namespace CVision.BLL.DTOs.Users;

public class UpdateProfileRequestDto
{
    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
}
