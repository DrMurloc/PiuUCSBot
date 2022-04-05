using Microsoft.EntityFrameworkCore;
using UCSBot.Infrastructure.Persistence.Entities;

namespace UCSBot.Infrastructure.Persistence;

public sealed class UcsBotDbContext : DbContext
{
    public UcsBotDbContext(DbContextOptions<UcsBotDbContext> options) : base(options)
    {
    }

    public DbSet<ChannelEntity> Channel { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChannelEntity>().ToContainer("UcsChannel")
            .HasPartitionKey(c => c.Id)
            .HasNoDiscriminator();
    }
}