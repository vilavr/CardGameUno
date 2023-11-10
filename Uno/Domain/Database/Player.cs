namespace Domain.Database;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Players")]
public class Player
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PlayerId { get; set; }

    [Required]
    [MaxLength(100)] 
    public required string PlayerNickname { get; set; }

    public List<Card>? PlayerCards { get; set; }
}
