using Dealytics.Domain.Dtos;
using Dealytics.Domain.Enums;
using Dealytics.Domain.ValueObjects;
using Dealytics.Features.Table;

namespace Dealytics.Features.PokerScenario;

public class RaiseOverLimpers
{
    private TableUseCases tableUseCases;

    public RaiseOverLimpers(TableUseCases tableUseCases)
    {
        this.tableUseCases = tableUseCases;
    }

    public async Task<string> ExecuteAsync(PokerScenarioRequest request)
    {
        try
        {
            var table = await tableUseCases.GetTable.ExecuteAsync(GameSituation.RaiseOverLimpers.GetDescription());
            if (table?.Value == null)
                throw new Exception("Table not found");

            var hand = request.HeroPosition == TablePosition.BigBlind && request.Limper != null
                ? GetBigBlindHand(table.Value, request)
                : GetDefaultHand(table.Value, request);

            return hand?.Action ?? string.Empty;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing OpenRaise scenario: {ex.Message}");
        }
    }

    private Hand? GetBigBlindHand(TableDTO table, PokerScenarioRequest request)
    {
        return table.Positions?
            .Where(w => w.HeroPosition == request.HeroPosition?.GetDescription() &&
                       w.Limper == request.Limper?.GetDescription())
            .FirstOrDefault()?.Hands
            .FirstOrDefault(f => f.Name == request.HandName);
    }

    private Hand? GetDefaultHand(TableDTO table, PokerScenarioRequest request)
    {
        return table.Positions?
            .Where(w => w.HeroPosition == request.HeroPosition?.GetDescription())
            .FirstOrDefault()?.Hands
            .FirstOrDefault(f => f.Name == request.HandName);
    }
}
