using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UCSBot.Infrastructure.Configuration;
using UCSBot.Infrastructure.Persistence.Entities;

namespace UCSBot.Infrastructure.Persistence;

public sealed class UcsBotDbContext : DbContext
{
    private readonly CosmosConfiguration _config;

    public UcsBotDbContext(DbContextOptions<UcsBotDbContext> options, IOptions<CosmosConfiguration> configOptions) :
        base(options)
    {
        _config = configOptions.Value;
    }

    public DbSet<ChannelEntity> Channel { get; set; }
    public DbSet<ChartEntity> Chart { get; set; }
    public DbSet<ChartMessageEntity> ChartMessage { get; set; }
    public DbSet<UserMessageCategoryEntity> UserMessageCategory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChannelEntity>().ToContainer(_config.ChannelContainerName)
            .HasPartitionKey(c => c.Id)
            .HasNoDiscriminator();

        modelBuilder.Entity<ChartEntity>().ToContainer(_config.ChartContainerName)
            .HasPartitionKey(c => c.SongName)
            .HasNoDiscriminator();

        modelBuilder.Entity<ChartMessageEntity>().ToContainer(_config.ChartMessageContainerName)
            .HasPartitionKey(c => c.Id)
            .HasNoDiscriminator();

        modelBuilder.Entity<UserMessageCategoryEntity>().ToContainer(_config.UserMessageCategoryContainerName)
            .HasPartitionKey(c => c.Category)
            .HasNoDiscriminator()
            .HasKey(c => new { c.Category, c.MessageId, c.UserId });
    }
}