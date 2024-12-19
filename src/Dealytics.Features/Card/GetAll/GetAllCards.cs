﻿using Ardalis.Result;
using Dealytics.Domain.Dtos;
using Dealytics.Domain.Mappers;
using Marten;

namespace Dealytics.Features.Card.GetAll;

public class GetAllCards
{
    private IDocumentStore _documentStore;

    public GetAllCards(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<Result<List<CardDTO>?>> ExecuteAsync()
    {
        try
        {
            using var session = _documentStore.QuerySession();
            var img = await session.Query<Domain.Entities.Card>().ToListAsync();
            if (img == null || !img.Any())
                return Result<List<CardDTO>?>.NotFound();

            return Result<List<CardDTO>?>.Success(img.Select(card => card.ToDto()).ToList());
        }
        catch (Exception ex)
        {
            return Result<List<CardDTO>?>.CriticalError(ex.Message);
        }
    }

}
