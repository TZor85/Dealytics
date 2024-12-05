using Ardalis.Result;
using Marten;

namespace Dealytics.Features.Regions.CreateAll;

public class CreateAllRegions
{
    readonly IDocumentStore _documentStore;

    public CreateAllRegions(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<Result> ExecuteAsync(CreateAllRegionsRequest request, CancellationToken ct = default)
    {
        try
        {
            using var session = _documentStore.LightweightSession();
            foreach (var region in request.Regions)
            {
                session.Store(region);
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
