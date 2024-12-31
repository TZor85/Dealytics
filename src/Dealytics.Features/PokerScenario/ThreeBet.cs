using Dealytics.Domain.Enums;
using Dealytics.Domain.ValueObjects;
using Dealytics.Features.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dealytics.Features.PokerScenario;

public class ThreeBet
{
    private TableUseCases tableUseCases;

    public ThreeBet(TableUseCases tableUseCases)
    {
        this.tableUseCases = tableUseCases;
    }

    public async Task<string> ExecuteAsync(PokerScenarioRequest request)
    {
        try
        {
            var table = await tableUseCases.GetTable.ExecuteAsync(GameSituation.ThreeBet.GetDescription());
            if (table?.Value == null)
                throw new Exception("Table not found");

            var hand = table.Value.Positions?
                                .Where(w => w.HeroPosition == request.HeroPosition?.GetDescription() &&
                                            w.OpenRaiser == request?.OpenRaiser?.GetDescription())
                                .FirstOrDefault()?.Hands
                                .FirstOrDefault(f => f.Name == request.HandName);

            return hand?.Action ?? "Fold";
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing ThreeBet scenario: {ex.Message}");
        }
    }
}
