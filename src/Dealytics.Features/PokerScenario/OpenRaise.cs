using Dealytics.Domain.Enums;
using Dealytics.Features.Table;

namespace Dealytics.Features.PokerScenario;

public class OpenRaise
{
    private TableUseCases tableUseCases;

    public OpenRaise(TableUseCases tableUseCases)
    {
        this.tableUseCases = tableUseCases;
    }

    public async Task<string> ExecuteAsync(PokerScenarioRequest request)
    {
        try
        {
            var table = await tableUseCases.GetTable.ExecuteAsync(GameSituation.OpenRaise.GetDescription());
            if (table?.Value == null)
                throw new Exception("Table not found");

            var hand = table.Value.Positions?
                        .Where(w => w.HeroPosition == request.HeroPosition?.GetDescription())
                        .FirstOrDefault()?.Hands
                        .FirstOrDefault(f => f.Name == request.HandName);

            return hand?.Action ?? "Fold";
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing OpenRaise scenario: {ex.Message}");
        }
    }
}
