namespace CVision.BLL.DTOs.Analytics;

public class AggregateSalaryResponse
{
    public LocationShortInfo QueryLocation { get; set; } = new LocationShortInfo();

    public ICollection<SalaryRecord> Results { get; set; } = new List<SalaryRecord>();
}
