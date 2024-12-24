using Dealytics.Domain.Entities;
using System.Drawing;

namespace Dealytics.Domain.ValueObjects;

public class DataRegions
{
    // Names
    public string? P1Name { get; set; } = string.Empty;
    public string? P2Name { get; set; } = string.Empty;
    public string? P3Name { get; set; } = string.Empty;
    public string? P4Name { get; set; } = string.Empty;
    public string? P5Name { get; set; } = string.Empty;

    // Board
    public Card? Card1 { get; set; } = new Card { Id = string.Empty };
    public Card? Card2 { get; set; } = new Card { Id = string.Empty };
    public Card? Card3 { get; set; } = new Card { Id = string.Empty };
    public Card? Card4 { get; set; } = new Card { Id = string.Empty };
    public Card? Card5 { get; set; } = new Card { Id = string.Empty };

    // Dealer
    public bool P0Dealer { get; set; }
    public bool P1Dealer { get; set; }
    public bool P2Dealer { get; set; }
    public bool P3Dealer { get; set; }
    public bool P4Dealer { get; set; }
    public bool P5Dealer { get; set; }

    // Bets
    public decimal P1Bet { get; set; }
    public decimal P2Bet { get; set; }
    public decimal P3Bet { get; set; }
    public decimal P4Bet { get; set; }
    public decimal P5Bet { get; set; }

    // Playing
    public bool P1Playing { get; set; }
    public bool P2Playing { get; set; }
    public bool P3Playing { get; set; }
    public bool P4Playing { get; set; }
    public bool P5Playing { get; set; }

    // Empty
    public bool P1Empty { get; set; }
    public bool P2Empty { get; set; }
    public bool P3Empty { get; set; }
    public bool P4Empty { get; set; }
    public bool P5Empty { get; set; }

    // SitOut
    public bool P1SitOut { get; set; }
    public bool P2SitOut { get; set; }
    public bool P3SitOut { get; set; }
    public bool P4SitOut { get; set; }
    public bool P5SitOut { get; set; }

    // Table
    public bool IsFlop { get; set; }
    public string? TableHand { get; set; } = string.Empty;
    public string? TableName { get; set; } = string.Empty;
    public decimal Pot { get; set; }

    // User
    public decimal U0Bet { get; set; }
    public string? U0CardFace0 { get; set; } = string.Empty;
    public string? U0CardFace1 { get; set; } = string.Empty;
    public bool UAction { get; set; }
}
