using Dealytics.Domain.Enum;
using Dealytics.Domain.Entities;

namespace Dealytics.Domain.ValueObjects;

public record BettingRound(BettingRoundType Type, List<BettingAction> Actions, List<Card> CommunityCards);