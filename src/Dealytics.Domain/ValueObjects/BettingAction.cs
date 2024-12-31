using Dealytics.Domain.Enums;

namespace Dealytics.Domain.ValueObjects;

public record BettingAction(string Player, ActionType Action, decimal Amount, DateTime Date);
