namespace CVision.BLL.DTOs.Analytics;

public class PayPercentiles
{
    public ICollection<PercentileItem> Percentiles { get; set; } = new List<PercentileItem>();
}
