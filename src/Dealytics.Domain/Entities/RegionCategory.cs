namespace Dealytics.Domain.Entities
{
    public class RegionCategory
    {
        public required string Id { get; set; }
        public List<ValueObjects.Region>? Regions { get; set; }
    }
}
