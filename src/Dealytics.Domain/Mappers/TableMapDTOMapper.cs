using Dealytics.Domain.Dtos;

namespace Dealytics.Domain.Mappers;

public static class TableMapDTOMapper
{
    public static TableMapDTO ToDto(this Entities.TableMap tableMap)
    {
        var regions = new List<RegionCategoryDTO>();

        if (tableMap.Regions != null)
        {
            foreach (var region in tableMap.Regions)
            {
                regions.Add(region.ToDto());
            }
        }

        return new TableMapDTO
        {
            Name = tableMap.Id,
            Regions = regions
        };
    }

    public static Entities.TableMap ToEntity(this TableMapDTO tableMap)
    {
        var regions = new List<Entities.RegionCategory>();

        if (tableMap.Regions != null)
        {
            foreach (var region in tableMap.Regions)
            {
                regions.Add(region.ToEntity());
            }
        }

        return new Entities.TableMap
        {
            Id = tableMap.Name,
            Regions = regions
        };
    }
}
