namespace Domain.Database;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Cards")]
public class Card
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CardId { get; set; }

    [Required]
    public required string CardColor { get; set; }

    [Required]
    public required string CardValue { get; set; }

    [Required]
    public int CardScore { get; set; }
    
    public List<PlayerCard>? PlayerCards { get; set; }
    
    // Navigation property for the many-to-many relationship with Game via GameCard
    public required List<GameCard> GameCards { get; set; }
}
