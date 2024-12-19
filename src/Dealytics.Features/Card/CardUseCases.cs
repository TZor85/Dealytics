using Dealytics.Features.Card.CreateAll;
using Dealytics.Features.Card.Get;
using Dealytics.Features.Card.GetAll;
using Dealytics.Features.InitialConfig.Card;

namespace Dealytics.Features.Card;

public record CardUseCases(CreateAllCards CreateAllCards, GetCard GetCard, GetAllCards GetAllCards, LoadAllCards LoadAllCards);

