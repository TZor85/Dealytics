using Dealytics.Features.Card.CreateAll;
using Dealytics.Features.InitialConfig.Card;

namespace Dealytics.Features.Card;

public record CardUseCases(CreateAllCards CreateAllCards, GetAllCards GetAllCards);

