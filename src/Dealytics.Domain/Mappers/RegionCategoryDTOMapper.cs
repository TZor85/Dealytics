using Dealytics.Domain.Dtos;

namespace Dealytics.Domain.Mappers;

public static class RegionCategoryDTOMapper
{
    public static RegionCategoryDTO ToDto(this Entities.RegionCategory region)
    {
        return new RegionCategoryDTO
        {
            Name = region.Id,
            Regions = region.Regions
        };
    }

    public static Entities.RegionCategory ToEntity(this RegionCategoryDTO region)
    {
        return new Entities.RegionCategory
        {
            Id = region.Name,
            Regions = region.Regions
        };
    }
}
