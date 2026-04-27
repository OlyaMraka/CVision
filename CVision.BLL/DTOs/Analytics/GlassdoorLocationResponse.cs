namespace CVision.BLL.DTOs.Analytics;

public class GlassdoorLocationResponse
{
    public ICollection<LocationItem> Data { get; set; } = new List<LocationItem>();

    public bool Status { get; set; }

    public string Message { get; set; } = string.Empty;
}