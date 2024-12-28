using Ardalis.Result;
using Marten;

namespace Dealytics.Features.Table.Create;

public class CreateTable
{
    readonly IDocumentStore _documentStore;

    public CreateTable(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<Result> ExecuteAsync(CreateTableRequest request, CancellationToken ct = default)
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
