namespace Dealytics.Domain.Entities;

public class TableMap
{
    public required string Id { get; set; }
    public List<RegionCategory>? Regions { get; set; } = [];
}
