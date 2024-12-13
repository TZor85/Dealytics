using Dealytics.Features.Regions.CreateAll;
using Dealytics.Features.Regions.Update;

namespace Dealytics.Features.Regions;

public record RegionUseCases(CreateAllRegions CreateAllRegions, UpdateRegion UpdateRegion);
