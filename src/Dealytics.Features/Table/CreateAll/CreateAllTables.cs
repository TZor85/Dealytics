using Ardalis.Result;
using Marten;

namespace Dealytics.Features.Table.CreateAll;

public class CreateAllTables
{
    readonly IDocumentStore _documentStore;

    public CreateAllTables(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<Result> ExecuteAsync(CreateAllTablesRequest request, CancellationToken ct = default)
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
