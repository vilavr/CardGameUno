using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Database;

[Table("PlayerCards")]
public class PlayerCard
{
    public int PlayerId { get; set; }
    public required Player Player { get; set; }

    public int CardId { get; set; }
    public required Card Card { get; set; }
}
