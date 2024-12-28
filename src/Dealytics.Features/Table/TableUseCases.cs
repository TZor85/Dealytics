using Dealytics.Features.InitialConfig.Table;
using Dealytics.Features.Table.Create;
using Dealytics.Features.Table.CreateAll;
using Dealytics.Features.Table.Get;

namespace Dealytics.Features.Table;

public record TableUseCases(CreateTable CreateAction, CreateAllTables CreateAllActions, GetTable GetTable, GetAllTables GetAllActions);
