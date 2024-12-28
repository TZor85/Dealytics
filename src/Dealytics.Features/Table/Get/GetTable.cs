using Ardalis.Result;
using Dealytics.Domain.Dtos;
using Dealytics.Domain.Mappers;
using Marten;

namespace Dealytics.Features.Table.Get
{
    public class GetTable
    {
        private IDocumentStore _documentStore;

        public GetTable(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task<Result<TableDTO?>> ExecuteAsync(string name)
        {
            try
            {
                using var session = _documentStore.QuerySession();
                
                var table = await session.Query<Domain.Entities.Table>()
                        .FirstOrDefaultAsync(x => x.Id == name);
                
                if (table == null)
                    return Result<TableDTO?>.NotFound();
                return Result<TableDTO?>.Success(table.ToDto());
            }
            catch (Exception ex)
            {
                return Result<TableDTO?>.CriticalError(ex.Message);
            }
        }
    }
}
