namespace CVision.BLL.DTOs.Contacts;

public class UserSearchResultDto
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsContact { get; set; }
}
