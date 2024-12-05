namespace Dealytics.Domain.ValueObjects;

public record Position(string Name, string? OpenRaiser, string? ThreeBetPosition, string? Limper, string? Caller, string? Squeezer, int? BetSize, bool? IsGreater, bool? RaiserFolds, List<Hand> Hands);
