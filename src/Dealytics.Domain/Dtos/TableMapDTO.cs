namespace Dealytics.Domain.Dtos;

public class TableMapDTO
{
    public required string Name { get; set; }
    public List<RegionCategoryDTO>? Regions { get; set; }
}
