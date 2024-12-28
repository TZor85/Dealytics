using Dealytics.Features.Card;
using Dealytics.Features.Card.CreateAll;
using Dealytics.Features.Card.Get;
using Dealytics.Features.Card.GetAll;
using Dealytics.Features.InitialConfig.Card;
using Dealytics.Features.InitialConfig.Region;
using Dealytics.Features.InitialConfig.Table;
using Dealytics.Features.InitialConfig.TableMap;
using Dealytics.Features.Regions;
using Dealytics.Features.Regions.CreateAll;
using Dealytics.Features.Regions.Update;
using Dealytics.Features.Table;
using Dealytics.Features.Table.Create;
using Dealytics.Features.Table.CreateAll;
using Dealytics.Features.Table.Get;
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
            .AddScoped<GetCard>()
            .AddScoped<GetAllCards>()
            .AddScoped<CardUseCases>()

            .AddScoped<CreateTable>()
            .AddScoped<CreateAllTables>()
            .AddScoped<GetTable>()
            .AddScoped<TableUseCases>()

            .AddScoped<GetAllRegions>()
            .AddScoped<LoadAllCards>()
            .AddScoped<GetAllTableMaps>()
            .AddScoped<GetAllTables>();
    }
}
