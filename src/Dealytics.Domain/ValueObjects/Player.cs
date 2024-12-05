using Dealytics.Domain.Enum;
using Dealytics.Domain.Entities;

namespace Dealytics.Domain.ValueObjects;

public record Player(string Name, decimal? CurrentBet, decimal? CurrentStack,
                        PlayerStatus Status, TablePosition Position, List<Card>? Cards);