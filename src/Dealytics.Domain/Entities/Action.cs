using Dealytics.Domain.ValueObjects;

namespace Dealytics.Domain.Entities;

public class Action
{
    public required string Id { get; set; }
    public List<Position>? Positions { get; set; }
}
