namespace Dealytics.Domain.ValueObjects;

public record Position(string Name, string HeroPosition, string? OpenRaiser, string? ThreeBetPosition, string? Limper, string? Caller, string? Squeezer, decimal? BetSize, bool? IsGreater, bool? RaiserFolds, List<Hand> Hands);
