using System.ComponentModel;

namespace Domain.Database;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("GameSettings")]
public class GameSetting
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [DefaultValue("Default")]
    public string FileName { get; set; } = "Default"; // Default value for FileName

    [Required]
    [MaxLength(100)] 
    public required string SettingName { get; set; }

    [Required]
    public required string SettingValue { get; set; } 

    // Foreign key for Game
    public int GameId { get; set; }

    // Navigation property; it can be null as user can just precreate settings for the game
    public Game? Game { get; set; }
}
