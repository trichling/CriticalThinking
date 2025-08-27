using CriticalThinkingGame.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace CriticalThinkingGame.ApiService.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    public DbSet<LogicalFallacy> LogicalFallacies { get; set; }
    public DbSet<GameText> GameTexts { get; set; }
    public DbSet<TextFallacy> TextFallacies { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<GameSessionFallacy> GameSessionFallacies { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<LogicalFallacyTranslation> LogicalFallacyTranslations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure LogicalFallacy
        modelBuilder.Entity<LogicalFallacy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Difficulty).IsRequired();
        });

        // Configure GameText
        modelBuilder.Entity<GameText>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Difficulty).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Language)
                .WithMany(e => e.GameTexts)
                .HasForeignKey(e => e.LanguageId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure TextFallacy
        modelBuilder.Entity<TextFallacy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StartIndex).IsRequired();
            entity.Property(e => e.EndIndex).IsRequired();

            entity.HasOne(e => e.GameText)
                .WithMany(e => e.TextFallacies)
                .HasForeignKey(e => e.GameTextId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.LogicalFallacy)
                .WithMany(e => e.TextFallacies)
                .HasForeignKey(e => e.LogicalFallacyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure GameSession
        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PlayerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Difficulty).IsRequired();
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.Score).IsRequired();
            entity.Property(e => e.IsCompleted).IsRequired();

            entity.HasOne(e => e.GameText)
                .WithMany(e => e.GameSessions)
                .HasForeignKey(e => e.GameTextId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure GameSessionFallacy
        modelBuilder.Entity<GameSessionFallacy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IsCorrect).IsRequired();
            entity.Property(e => e.SelectedAt).IsRequired();

            entity.HasOne(e => e.GameSession)
                .WithMany(e => e.SelectedFallacies)
                .HasForeignKey(e => e.GameSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.TextFallacy)
                .WithMany(e => e.GameSessionFallacies)
                .HasForeignKey(e => e.TextFallacyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Language
        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.NativeName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IsDefault).IsRequired();

            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Configure LogicalFallacyTranslation
        modelBuilder.Entity<LogicalFallacyTranslation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);

            entity.HasOne(e => e.LogicalFallacy)
                .WithMany(e => e.Translations)
                .HasForeignKey(e => e.LogicalFallacyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Language)
                .WithMany(e => e.LogicalFallacyTranslations)
                .HasForeignKey(e => e.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: one translation per fallacy per language
            entity.HasIndex(e => new { e.LogicalFallacyId, e.LanguageId }).IsUnique();
        });
    }
}
