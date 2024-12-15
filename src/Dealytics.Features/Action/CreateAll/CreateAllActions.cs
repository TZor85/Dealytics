using Ardalis.Result;
using Marten;

namespace Dealytics.Features.Action.CreateAll;

public class CreateAllActions
{
    readonly IDocumentStore _documentStore;

    public CreateAllActions(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<Result> ExecuteAsync(CreateAllActionsRequest request, CancellationToken ct = default)
    {
        try
        {
            using var session = _documentStore.LightweightSession();
            foreach (var action in request.Actions)
            {
                session.Store(action);
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
