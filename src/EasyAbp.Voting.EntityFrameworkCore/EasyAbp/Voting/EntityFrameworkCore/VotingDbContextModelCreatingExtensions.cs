using EasyAbp.Voting.Activities;
using EasyAbp.Voting.Players;
using EasyAbp.Voting.Rules;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace EasyAbp.Voting.EntityFrameworkCore;

public static class VotingDbContextModelCreatingExtensions
{
    public static void ConfigureVoting(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<Activity>(b =>
        {
            b.ToTable(VotingDbProperties.DbTablePrefix + "Activities", VotingDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(p => p.ActivityName).HasMaxLength(ActivityConsts.MaxActivityNameLength).IsRequired();
            b.Property(p => p.VotesUnit).HasMaxLength(ActivityConsts.MaxVotesUnitLength).IsRequired();

            b.HasMany(p => p.Banners).WithOne().HasForeignKey(p => p.ActivityId);
            b.HasMany(p => p.Groups).WithOne().HasForeignKey(p => p.ActivityId);
        });

        builder.Entity<Banner>(b =>
        {
            b.ToTable(VotingDbProperties.DbTablePrefix + "Banners", VotingDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(p => p.Url).IsRequired();

            b.HasIndex(p => p.ActivityId);
        });

        builder.Entity<Group>(b =>
        {
            b.ToTable(VotingDbProperties.DbTablePrefix + "Groups", VotingDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(p => p.Name).HasMaxLength(GroupConsts.MaxNameLength).IsRequired();
            b.Property(p => p.Description).HasMaxLength(GroupConsts.MaxDescriptionLength);

            b.HasIndex(p => p.ActivityId);
        });

        builder.Entity<Player>(b =>
        {
            b.ToTable(VotingDbProperties.DbTablePrefix + "Players", VotingDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(p => p.Name).HasMaxLength(PlayerConsts.MaxNameLength).IsRequired();
            b.Property(p => p.RejectReason).HasMaxLength(PlayerConsts.MaxRejectReasonLength);
            b.Property(p => p.FormContent).IsRequired();

            b.HasIndex(p => new { p.ActivityId, p.GroupId });
        });

        builder.Entity<PlayerVotes>(b =>
        {
            b.ToTable(VotingDbProperties.DbTablePrefix + "PlayerVotes", VotingDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.HasKey(p => new { p.PlayerId });
        });

        builder.Entity<Rule>(b =>
        {
            b.ToTable(VotingDbProperties.DbTablePrefix + "Rules", VotingDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.Property(p => p.Name).HasMaxLength(RuleConsts.MaxNameLength).IsRequired();

            b.HasIndex(p => new { p.ActivityId, p.Name }).IsUnique();
        });
    }
}
