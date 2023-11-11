using Microsoft.EntityFrameworkCore;
using Domain.Database; // Replace with your actual namespace where the entity classes are

namespace DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Card> Cards { get; set; } = null!;
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<GameCard> GameCards { get; set; } = null!;
    public DbSet<GameSetting> GameSettings { get; set; } = null!;
    public DbSet<Player> Players { get; set; } = null!;
    public DbSet<PlayerCard> PlayerCards { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraint for GameSetting.FileName
        modelBuilder.Entity<GameSetting>()
            .HasIndex(gs => new { gs.FileName, gs.SettingName })
            .IsUnique();

        // Unique constraint for Player.PlayerNickname
        modelBuilder.Entity<Player>()
            .HasIndex(p => p.PlayerNickname)
            .IsUnique();

        // Configuring PlayerCard (many-to-many relationship between Player and Card)
        modelBuilder.Entity<PlayerCard>()
            .HasKey(pc => new { pc.PlayerId, pc.CardId });

        modelBuilder.Entity<PlayerCard>()
            .HasOne(pc => pc.Player)
            .WithMany(p => p.PlayerCards)
            .HasForeignKey(pc => pc.PlayerId);

        modelBuilder.Entity<PlayerCard>()
            .HasOne(pc => pc.Card)
            .WithMany(c => c.PlayerCards)
            .HasForeignKey(pc => pc.CardId);

        // Configuring GameCard (many-to-many relationship between Game and Card)
        modelBuilder.Entity<GameCard>()
            .HasKey(gc => new { gc.GameId, gc.CardId });

        modelBuilder.Entity<GameCard>()
            .HasOne(gc => gc.Game)
            .WithMany(g => g.GameCards)
            .HasForeignKey(gc => gc.GameId);

        modelBuilder.Entity<GameCard>()
            .HasOne(gc => gc.Card)
            .WithMany(c => c.GameCards)
            .HasForeignKey(gc => gc.CardId);

        // Configuring one-to-many relationship between Game and GameSetting
        modelBuilder.Entity<GameSetting>()
            .HasOne(gs => gs.Game)
            .WithMany(g => g.GameSettings)
            .HasForeignKey(gs => gs.GameId);

        // Configuring one-to-one relationship between Game and TopCard
        modelBuilder.Entity<Game>()
            .HasOne(g => g.TopCard)
            .WithOne()
            .HasForeignKey<Game>(g => g.TopCardId);
    }
}
