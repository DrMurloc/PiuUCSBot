using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UCSBot.Infrastructure.Configuration;
using UCSBot.Infrastructure.Persistence.Entities;

namespace UCSBot.Infrastructure.Persistence;

public sealed class UcsBotDbContext : DbContext
{
    private readonly CosmosConfiguration _config;

    public UcsBotDbContext(DbContextOptions<UcsBotDbContext> options, IOptions<CosmosConfiguration> options) :
        base(options)
    {
        _config = options.Value;
    }

    public DbSet<ChannelEntity> Channel { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChannelEntity>().ToContainer(_config.ChannelContainerName)
            .HasPartitionKey(c => c.Id)
            .HasNoDiscriminator();
    }
}