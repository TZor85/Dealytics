using Dealytics.Domain.Dtos;
using Dealytics.Domain.Enums;
using Dealytics.Domain.ValueObjects;
using Dealytics.Features.Table;

namespace Dealytics.Features.PokerScenario;

public class BigBlindVsSmallBlind
{
    private TableUseCases tableUseCases;

    public BigBlindVsSmallBlind(TableUseCases tableUseCases)
    {
        this.tableUseCases = tableUseCases;
    }

    public async Task<string> ExecuteAsync(PokerScenarioRequest request)
    {
        try
        {
            var table = await tableUseCases.GetTable.ExecuteAsync(GameSituation.BigBlindVsSmallBlind.GetDescription());
            if (table?.Value == null)
                throw new Exception("Table not found");

            var hand = request.HeroPosition == TablePosition.BigBlind && request.ThreeBetPosition != null
                ? SmallblindThreeBet(table.Value, request)
                : BigBlindOnlyCall(table.Value, request);

            return hand?.Action ?? "Fold";

        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing BigBlindVsSmallBlind scenario: {ex.Message}");
        }
    }

    private Hand? SmallblindThreeBet(TableDTO table, PokerScenarioRequest request)
    {
        return table.Positions?
            .Where(w => w.HeroPosition == request.HeroPosition?.GetDescription() &&
                       w.ThreeBetPosition == request.ThreeBetPosition?.GetDescription())
            .FirstOrDefault()?.Hands
            .FirstOrDefault(f => f.Name == request.HandName);
    }

    private Hand? BigBlindOnlyCall(TableDTO table, PokerScenarioRequest request)
    {
        return table.Positions?
            .Where(w => w.HeroPosition == request.HeroPosition?.GetDescription() &&
                       w.ThreeBetPosition is null)
            .FirstOrDefault()?.Hands
            .FirstOrDefault(f => f.Name == request.HandName);
    }
}
