using Dealytics.Features.Action.Create;
using Dealytics.Features.Action.CreateAll;
using Dealytics.Features.InitialConfig.Action;

namespace Dealytics.Features.Action;

public record ActionUseCases(CreateAction CreateAction, CreateAllActions CreateAllActions, GetAllActions GetAllActions);
