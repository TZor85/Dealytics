namespace Dealytics.Domain.Dtos;

public class RegionCategoryDTO
{
    public required string Name { get; set; }
    public List<ValueObjects.Region>? Regions { get; set; }
}
