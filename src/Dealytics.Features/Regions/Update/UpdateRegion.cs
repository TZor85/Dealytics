using Ardalis.Result;
using Marten;

namespace Dealytics.Features.Regions.Update;

public class UpdateRegion
{
    readonly IDocumentStore _documentStore;

    public UpdateRegion(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<Result> ExecuteAsync(UpdateRegionRequest request, CancellationToken ct = default)
    {
        try
        {
            using var session = _documentStore.LightweightSession();
            var region = await session.LoadAsync<Domain.Entities.RegionCategory>(request.Category, ct);
            if (region == null)
                return Result.NotFound();

            var regionToRemove = region.Regions?.FirstOrDefault(r => r.Name == request.Name);
            if (regionToRemove != null)
            {
                region.Regions?.Remove(regionToRemove);
            }

            var regionCategory = new Domain.ValueObjects.Region
            {
                Category = request.Category,
                Name = request.Name,
                PosX = request.PosX,
                PosY = request.PosY,
                Width = request.Width,
                Height = request.Height,
                IsHash = regionToRemove?.IsHash,
                IsColor = regionToRemove?.IsColor,
                IsBoard = regionToRemove?.IsBoard,
                Color = request?.Color,
                IsOnlyNumber = regionToRemove?.IsOnlyNumber,
                InactiveUmbral = request?.InactiveUmbral,
                Umbral = request?.Umbral
            };

            region.Regions?.Add(regionCategory);

            session.Store(region);
            await session.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.CriticalError(ex.Message);
        }
    }
}
