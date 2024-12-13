using Ardalis.Result;
using Marten;

namespace Dealytics.Features.Action.Create;

public class CreateAction
{
    readonly IDocumentStore _documentStore;

    public CreateAction(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<Result> ExecuteAsync(CreateActionRequest request, CancellationToken ct = default)
    {
        try
        {
            using var session = _documentStore.LightweightSession();

            session.Store(request.Action);
            await session.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.CriticalError(ex.Message);
        }
    }

}
