using CriticalThinking.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CriticalThinking.Backend.Data;

public class CriticalThinkingContext : DbContext
{
    public CriticalThinkingContext(DbContextOptions<CriticalThinkingContext> options) : base(options)
    {
    }

    public DbSet<LogicalFallacy> LogicalFallacies { get; set; } = null!;
    public DbSet<TextBlock> TextBlocks { get; set; } = null!;
    public DbSet<GameText> GameTexts { get; set; } = null!;
    public DbSet<GameTextFallacy> GameTextFallacies { get; set; } = null!;
    public DbSet<GameSession> GameSessions { get; set; } = null!;
    public DbSet<GameAnswer> GameAnswers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure all DateTime properties to use UTC
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                        v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
                        v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime()) : v,
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v));
                }
            }
        }

        // Configure enums
        modelBuilder.Entity<LogicalFallacy>()
            .Property(e => e.Difficulty)
            .HasConversion<string>();

        modelBuilder.Entity<GameText>()
            .Property(e => e.Difficulty)
            .HasConversion<string>();

        modelBuilder.Entity<GameSession>()
            .Property(e => e.Difficulty)
            .HasConversion<string>();

        modelBuilder.Entity<GameAnswer>()
            .Property(e => e.AnswerType)
            .HasConversion<string>();

        // Configure relationships
        modelBuilder.Entity<TextBlock>()
            .HasOne(tb => tb.Fallacy)
            .WithMany(lf => lf.TextBlocks)
            .HasForeignKey(tb => tb.FallacyId);

        modelBuilder.Entity<GameTextFallacy>()
            .HasOne(gtf => gtf.GameText)
            .WithMany(gt => gt.GameTextFallacies)
            .HasForeignKey(gtf => gtf.GameTextId);

        modelBuilder.Entity<GameTextFallacy>()
            .HasOne(gtf => gtf.Fallacy)
            .WithMany(lf => lf.GameTextFallacies)
            .HasForeignKey(gtf => gtf.FallacyId);

        modelBuilder.Entity<GameSession>()
            .HasOne(gs => gs.GameText)
            .WithMany(gt => gt.GameSessions)
            .HasForeignKey(gs => gs.GameTextId);

        modelBuilder.Entity<GameAnswer>()
            .HasOne(ga => ga.Session)
            .WithMany(gs => gs.GameAnswers)
            .HasForeignKey(ga => ga.SessionId);

        modelBuilder.Entity<GameAnswer>()
            .HasOne(ga => ga.Fallacy)
            .WithMany(lf => lf.GameAnswers)
            .HasForeignKey(ga => ga.FallacyId);

        // Configure unique constraints
        modelBuilder.Entity<LogicalFallacy>()
            .HasIndex(lf => lf.Key)
            .IsUnique();

        modelBuilder.Entity<GameTextFallacy>()
            .HasIndex(gtf => new { gtf.GameTextId, gtf.FallacyId })
            .IsUnique();

        // Configure table names to match PostgreSQL conventions
        modelBuilder.Entity<LogicalFallacy>().ToTable("logical_fallacies");
        modelBuilder.Entity<TextBlock>().ToTable("text_blocks");
        modelBuilder.Entity<GameText>().ToTable("game_texts");
        modelBuilder.Entity<GameTextFallacy>().ToTable("game_text_fallacies");
        modelBuilder.Entity<GameSession>().ToTable("game_sessions");
        modelBuilder.Entity<GameAnswer>().ToTable("game_answers");

        // Configure column names to match PostgreSQL conventions
        modelBuilder.Entity<LogicalFallacy>().Property(e => e.Id).HasColumnName("id");
        modelBuilder.Entity<LogicalFallacy>().Property(e => e.Name).HasColumnName("name");
        modelBuilder.Entity<LogicalFallacy>().Property(e => e.Key).HasColumnName("key");
        modelBuilder.Entity<LogicalFallacy>().Property(e => e.Description).HasColumnName("description");
        modelBuilder.Entity<LogicalFallacy>().Property(e => e.Difficulty).HasColumnName("difficulty");
        modelBuilder.Entity<LogicalFallacy>().Property(e => e.Example).HasColumnName("example");

        modelBuilder.Entity<TextBlock>().Property(e => e.Id).HasColumnName("id");
        modelBuilder.Entity<TextBlock>().Property(e => e.FallacyId).HasColumnName("fallacy_id");
        modelBuilder.Entity<TextBlock>().Property(e => e.Content).HasColumnName("content");
        modelBuilder.Entity<TextBlock>().Property(e => e.Context).HasColumnName("context");
        modelBuilder.Entity<TextBlock>().Property(e => e.PositionHint).HasColumnName("position_hint");

        modelBuilder.Entity<GameText>().Property(e => e.Id).HasColumnName("id");
        modelBuilder.Entity<GameText>().Property(e => e.Title).HasColumnName("title");
        modelBuilder.Entity<GameText>().Property(e => e.FullText).HasColumnName("full_text");
        modelBuilder.Entity<GameText>().Property(e => e.Difficulty).HasColumnName("difficulty");
        modelBuilder.Entity<GameText>().Property(e => e.TargetFallacyCount).HasColumnName("target_fallacy_count");

        modelBuilder.Entity<GameTextFallacy>().Property(e => e.Id).HasColumnName("id");
        modelBuilder.Entity<GameTextFallacy>().Property(e => e.GameTextId).HasColumnName("game_text_id");
        modelBuilder.Entity<GameTextFallacy>().Property(e => e.FallacyId).HasColumnName("fallacy_id");
        modelBuilder.Entity<GameTextFallacy>().Property(e => e.TextPositionStart).HasColumnName("text_position_start");
        modelBuilder.Entity<GameTextFallacy>().Property(e => e.TextPositionEnd).HasColumnName("text_position_end");

        modelBuilder.Entity<GameSession>().Property(e => e.Id).HasColumnName("id");
        modelBuilder.Entity<GameSession>().Property(e => e.PlayerName).HasColumnName("player_name");
        modelBuilder.Entity<GameSession>().Property(e => e.Difficulty).HasColumnName("difficulty");
        modelBuilder.Entity<GameSession>().Property(e => e.GameTextId).HasColumnName("game_text_id");
        modelBuilder.Entity<GameSession>().Property(e => e.StartedAt).HasColumnName("started_at");
        modelBuilder.Entity<GameSession>().Property(e => e.CompletedAt).HasColumnName("completed_at");
        modelBuilder.Entity<GameSession>().Property(e => e.TimeTakenSeconds).HasColumnName("time_taken_seconds");
        modelBuilder.Entity<GameSession>().Property(e => e.Score).HasColumnName("score");

        modelBuilder.Entity<GameAnswer>().Property(e => e.Id).HasColumnName("id");
        modelBuilder.Entity<GameAnswer>().Property(e => e.SessionId).HasColumnName("session_id");
        modelBuilder.Entity<GameAnswer>().Property(e => e.FallacyId).HasColumnName("fallacy_id");
        modelBuilder.Entity<GameAnswer>().Property(e => e.IsCorrect).HasColumnName("is_correct");
        modelBuilder.Entity<GameAnswer>().Property(e => e.AnswerType).HasColumnName("answer_type");
    }
}