namespace Domain.Database;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Games")]
public class Game
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string? SettingsFileName { get; set; } // Stores the setting file name

    // All other properties are nullable and null by default
    public List<Player>? Players { get; set; }
    public List<GameCard>? GameCards { get; set; }
    
    public int? TopCardId { get; set; }
    public Card? TopCard { get; set; }

    public bool? SpecialCardEffectApplied { get; set; }
    public int? CurrentPlayerTurn { get; set; }
    public int? CurrentRound { get; set; }
}
