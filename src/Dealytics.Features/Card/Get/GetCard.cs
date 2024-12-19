using Ardalis.Result;
using Dealytics.Domain.Dtos;
using Marten;
using Dealytics.Domain.Mappers;

namespace Dealytics.Features.Card.Get;

public class GetCard
{
    private IDocumentStore _documentStore;

    public GetCard(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<Result<CardDTO?>> ExecuteAsync(string base64)
    {
        try
        {
            using var session = _documentStore.QuerySession();
            var img = await session.Query<Domain.Entities.Card>()
                    .FirstOrDefaultAsync(x => x.ImageBase64 == base64);
            if (img == null)
                return Result<CardDTO?>.NotFound();
            return Result<CardDTO?>.Success(img.ToDto());
        }
        catch (Exception ex)
        {
            return Result<CardDTO?>.CriticalError(ex.Message);
        }
    }
}
