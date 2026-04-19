namespace CVision.DAL.Entities;

public class CvLookup
{
    public int Id { get; set; }

    public string LookupWord { get; set; } = string.Empty;

    public int CvId { get; set; }

    public virtual CV CV { get; set; } = null!;
}
