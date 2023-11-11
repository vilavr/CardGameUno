using Microsoft.EntityFrameworkCore;

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
    public string? GameFile { get; set; } // Can be null if the game hasn't been saved yet

    // Relationships
    public required List<Player> Players { get; set; }
    public required List<GameCard> GameCards { get; set; }
    // Navigation property for the one-to-many relationship with GameSetting
    public required List<GameSetting> GameSettings { get; set; }
    
    public int TopCardId { get; set; }
    public required Card TopCard { get; set; }

    public bool SpecialCardEffectApplied { get; set; }
    public int CurrentPlayerTurn { get; set; }
    public int CurrentRound { get; set; }
}
