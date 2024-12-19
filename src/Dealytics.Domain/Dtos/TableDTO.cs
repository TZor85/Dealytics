using Dealytics.Domain.ValueObjects;

namespace Dealytics.Domain.Dtos;

public class TableDTO
{
    public required string Name { get; set; }
    public List<Position>? Positions { get; set; }
}
