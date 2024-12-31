using Dealytics.Domain.Entities;
using Dealytics.Domain.Enums;

namespace Dealytics.Domain.ValueObjects;

public class Player
{
    public string? Id { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;
    public bool IsDealer { get; set; }
    public decimal Bet { get; set; }
    public bool IsPlaying { get; set; }
    public bool IsEmpty { get; set; }
    public bool IsSitOut { get; set; }
    public TablePosition Position { get; set; }

}

public class Board : Card
{    
    public BettingRoundType? BettingRoundType { get; set; }
}

public class UserData
{
    public decimal Bet { get; set; }
    public bool IsDealer { get; set; }
    public Card CardFace0 { get; set; } = new Card { Id = string.Empty };
    public Card CardFace1 { get; set; } = new Card { Id = string.Empty };
    public bool Action { get; set; }
    public TablePosition Position { get; set; }
}

public class TableData
{
    public bool IsFlop { get; set; }
    public string Hand { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Pot { get; set; }
}

public class DataRegions
{
    private const int MAX_PLAYERS = 5; // Sin contar P0

    public List<Player>? Players { get; set; }

    public List<Board>? BoardCards { get; set; }

    // Datos de la mesa
    public TableData? Table { get; set; }

    // Datos del usuario
    public UserData? User { get; set; }

    public DataRegions()
    {
        Players = Enumerable.Range(1, MAX_PLAYERS)
               .Select(i => new Player { Id = $"P{i}" })
               .ToList();

        BoardCards = Enumerable.Range(0, 5)
            .Select(_ => new Board { Id = string.Empty })
            .ToList();

        Table = new TableData();
        User = new UserData();
    }
}
