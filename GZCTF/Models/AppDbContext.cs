﻿using CTFServer.Models.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CTFServer.Models;

public class AppDbContext : IdentityDbContext<UserInfo>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<LogModel> Logs { get; set; } = default!;
    public DbSet<Submission> Submissions { get; set; } = default!;
    public DbSet<Challenge> Challenges { get; set; } = default!;
    public DbSet<Notice> Notices { get; set; } = default!;
    public DbSet<GameNotice> GameNotices { get; set; } = default!;
    public DbSet<GameEvent> GameEvents { get; set; } = default!;
    public DbSet<LocalFile> Files { get; set; } = default!;
    public DbSet<Game> Games { get; set; } = default!;
    public DbSet<Instance> Instances { get; set; } = default!;
    public DbSet<Participation> Participations { get; set; } = default!;
    public DbSet<Team> Teams { get; set; } = default!;
    public DbSet<FlagContext> FlagContexts { get; set; } = default!;
    public DbSet<Container> Containers { get; set; } = default!;
    public DbSet<Attachment> Attachments { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserInfo>(entity =>
        {
            entity.Property(e => e.Role)
                .HasConversion<int>();

            entity.Property(e => e.UserName)
                .HasMaxLength(12);

            entity.HasMany(e => e.Submissions)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.ActiveTeam)
                .WithMany()
                .HasForeignKey(e => e.ActiveTeamId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.OwnedTeam)
                .WithMany()
                .HasForeignKey(e => e.OwnedTeamId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Navigation(e => e.ActiveTeam).AutoInclude();
            entity.Navigation(e => e.OwnedTeam).AutoInclude();
        });

        builder.Entity<Game>(entity =>
        {
            entity.HasMany(e => e.GameEvents)
                .WithOne(e => e.Game)
                .HasForeignKey(e => e.GameId);

            entity.HasMany(e => e.GameNotices)
                .WithOne(e => e.Game)
                .HasForeignKey(e => e.GameId);

            entity.HasMany(e => e.Challenges)
                .WithOne(e => e.Game)
                .HasForeignKey(e => e.GameId);

            entity.HasMany(e => e.Submissions)
                .WithOne(e => e.Game)
                .HasForeignKey(e => e.GameId);

            entity.HasMany(e => e.Teams)
                .WithMany(e => e.Games)
                .UsingEntity<Participation>(
                    e => e.HasOne(e => e.Team)
                        .WithMany(e => e.Participations)
                        .HasForeignKey(e => e.TeamId),
                    e => e.HasOne(e => e.Game)
                        .WithMany(e => e.Participations)
                        .HasForeignKey(e => e.GameId),
                    e => e.HasIndex(e => new { e.TeamId, e.GameId })
                );
        });

        builder.Entity<Team>(entity =>
        {
            entity.HasMany(e => e.Members)
                .WithMany(e => e.Teams);

            entity.HasOne(e => e.Captain)
                .WithMany()
                .HasForeignKey(e => e.CaptainId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Participation>(entity =>
        {
            entity.Property(e => e.Status)
                .HasConversion<string>();

            entity.HasMany(e => e.Instances).WithOne();

            entity.Navigation(e => e.Game).AutoInclude();
            entity.Navigation(e => e.Team).AutoInclude();

            entity.HasMany(e => e.Submissions)
                .WithOne(e => e.Participation)
                .HasForeignKey(e => e.ParticipationId);

            entity.HasMany(e => e.Challenges)
                .WithMany(e => e.Teams)
                .UsingEntity<Instance>(
                    e => e.HasOne(e => e.Challenge)
                        .WithMany(c => c.Instances)
                        .HasForeignKey(e => e.ChallengeId),
                    e => e.HasOne(e => e.Participation)
                        .WithMany(e => e.Instances)
                        .HasForeignKey(e => e.ParticipationId)
                        .OnDelete(DeleteBehavior.Cascade),
                    e => e.HasKey(e => new { e.ChallengeId, e.ParticipationId })
                );
        });

        builder.Entity<Instance>(entity =>
        {
            entity.HasOne(e => e.FlagContext)
                .WithMany()
                .HasForeignKey(e => e.FlagId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Container)
                .WithOne(e => e.Instance)
                .HasForeignKey<Container>(e => e.InstanceId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.Navigation(e => e.Container).AutoInclude();
            entity.Navigation(e => e.Challenge).AutoInclude();

            entity.HasIndex(e => new { e.ParticipationId, e.ChallengeId });
        });

        builder.Entity<Container>(entity =>
        {
            entity.HasOne(e => e.Instance)
                .WithOne(e => e.Container)
                .HasForeignKey<Instance>(e => e.ContainerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.InstanceId);
        });

        builder.Entity<Challenge>(entity =>
        {
            entity.HasMany(e => e.Flags)
               .WithOne(e => e.Challenge)
               .HasForeignKey(e => e.ChallengeId);

            entity.HasMany(e => e.Submissions)
                .WithOne(e => e.Challenge)
                .HasForeignKey(e => e.ChallengeId);

            entity.HasOne(e => e.Attachment)
                .WithMany()
                .HasForeignKey(e => e.AttachmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.TestContainer)
                .WithMany()
                .HasForeignKey(e => e.TestContainerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Navigation(e => e.Attachment).AutoInclude();
            entity.Navigation(e => e.TestContainer).AutoInclude();

            entity.HasIndex(e => e.GameId);
        });

        builder.Entity<Submission>(entity =>
        {
            entity.Property(e => e.Status)
                .HasConversion<string>();

            entity.Navigation(e => e.Team).AutoInclude();
            entity.Navigation(e => e.User).AutoInclude();

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.TeamId, e.ChallengeId, e.GameId });
        });

        builder.Entity<FlagContext>(entity =>
        {
            entity.HasOne(e => e.Attachment)
                .WithMany()
                .HasForeignKey(e => e.AttachmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Navigation(e => e.Attachment).AutoInclude();

            entity.HasIndex(e => e.ChallengeId);
        });

        builder.Entity<Attachment>(entity =>
        {
            entity.HasOne(e => e.LocalFile)
                .WithMany()
                .HasForeignKey(e => e.LocalFileId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Navigation(e => e.LocalFile).AutoInclude();
        });

        builder.Entity<GameEvent>(entity =>
        {
            entity.HasOne(e => e.Team)
                .WithMany()
                .HasForeignKey(e => e.TeamId);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            entity.Navigation(e => e.Team).AutoInclude();
            entity.Navigation(e => e.User).AutoInclude();
        });
    }
}