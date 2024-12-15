using Dealytics.Features.Action;
using Dealytics.Features.Action.Create;
using Dealytics.Features.Action.CreateAll;
using Dealytics.Features.Card;
using Dealytics.Features.Card.CreateAll;
using Dealytics.Features.InitialConfig.Action;
using Dealytics.Features.InitialConfig.Card;
using Dealytics.Features.InitialConfig.Region;
using Dealytics.Features.InitialConfig.TableMap;
using Dealytics.Features.Regions;
using Dealytics.Features.Regions.CreateAll;
using Dealytics.Features.Regions.Update;
using Microsoft.Extensions.DependencyInjection;

namespace Dealytics.Features
{
    public static class Services
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services) =>
        services
            .AddScoped<CreateAllRegions>()
            .AddScoped<UpdateRegion>()
            .AddScoped<RegionUseCases>()

            .AddScoped<CreateAllCards>()
            .AddScoped<CardUseCases>()

            .AddScoped<CreateAction>()
            .AddScoped<CreateAllActions>()
            .AddScoped<ActionUseCases>()

            .AddScoped<GetAllRegions>()
            .AddScoped<GetAllCards>()
            .AddScoped<GetAllTableMaps>()
            .AddScoped<GetAllActions>();
    }
}
