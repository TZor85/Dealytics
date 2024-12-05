using Dealytics.Domain.ValueObjects;

namespace Dealytics.Domain.Dtos;

public class ActionDTO
{
    public required string Name { get; set; }
    public List<Position>? Positions { get; set; }
}
