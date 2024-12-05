using Ardalis.Result;
using Marten;

namespace Dealytics.Features.Card.CreateAll;

public class CreateAllCards
{
    readonly IDocumentStore _documentStore;

    public CreateAllCards(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<Result> ExecuteAsync(CreateAllCardsRequest request, CancellationToken ct = default)
    {
        try
        {
            using var session = _documentStore.LightweightSession();
            foreach (var card in request.Cards)
            {
                session.Store(card);
            }
            await session.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.CriticalError(ex.Message);
        }
    }
}
