using Dealytics.Domain.Enums;
using Dealytics.Domain.ValueObjects;

namespace Dealytics.Domain.Entities;

public class Hand
{
    public Guid Id { get; set; }
    public Guid TableId { get; set; }
    public List<Player>? Players { get; set; }
    public decimal Pot { get; set; }
    public HandStatus Status { get; set; }
    public List<BettingRound>? BettingRounds { get; set; }

}