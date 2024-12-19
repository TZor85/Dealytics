using Dealytics.Domain.Dtos;
using Dealytics.Domain.ValueObjects;

namespace Dealytics.Domain.Mappers;

public static class TableDTOMapper
{
    public static TableDTO ToDto(this Entities.Table action)
    {
        var actionDto = new TableDTO
        {
            Name = action.Id,
            Positions = new List<Position>()
        };

        if (action.Positions != null)
        {
            foreach (var item in action.Positions)
            {
                actionDto.Positions.Add(item);
            }
        }

        return actionDto;
    }
    public static Entities.Table ToEntity(this TableDTO action)
    {
        var actionEntity = new Entities.Table
        {
            Id = action.Name,
            Positions = new List<Position>()
        };
        if (action.Positions != null)
        {
            foreach (var item in action.Positions)
            {
                actionEntity.Positions.Add(item);
            }
        }
        return actionEntity;
    }

}
