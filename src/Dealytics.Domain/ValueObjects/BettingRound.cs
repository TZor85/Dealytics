using Dealytics.Domain.Entities;
using Dealytics.Domain.Enums;

namespace Dealytics.Domain.ValueObjects;

public record BettingRound(BettingRoundType Type, List<BettingAction> Actions, List<Card> CommunityCards);