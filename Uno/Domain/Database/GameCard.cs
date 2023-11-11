using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Database;

[Table("GameCards")]
public class GameCard
{
    public int GameId { get; set; }
    public required Game Game { get; set; }

    public int CardId { get; set; }
    public required Card Card { get; set; }

    public bool IsInDiscard { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsTopCard { get; set; }
}
