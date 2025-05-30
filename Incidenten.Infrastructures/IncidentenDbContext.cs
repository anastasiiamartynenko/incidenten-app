using Incidenten.Domain;
using Microsoft.EntityFrameworkCore;

namespace Incidenten.Infrastructures;

public class IncidentenDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public IncidentenDbContext(DbContextOptions<IncidentenDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure table names / constraints / etc.
        modelBuilder.Entity<User>().ToTable("users");
    }
}
