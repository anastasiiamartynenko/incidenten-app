using Incidenten.Domain;
using Incidenten.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Incidenten.Infrastructures;

public class IncidentenDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Incident> Incidents { get; set; }
    public DbSet<IncidentImage> IncidentImages { get; set; }
    
    public IncidentenDbContext(DbContextOptions<IncidentenDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure table names / constraints / etc.
        modelBuilder.Entity<User>()
            .ToTable("users")
            // Seed the table with the ANONYM user.
            .HasData(new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Role = UserRole.Anonym,
                CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
            });
        modelBuilder.Entity<Incident>()
            .ToTable("incidents")
            .HasOne(c => c.Reporter)
            .WithMany(u => u.ReportedIncidents)
            .HasForeignKey(c => c.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Incident>()
            .ToTable("incidents")
            .HasOne(c => c.Executor)
            .WithMany(u => u.ResolvedIncidents)
            .HasForeignKey(c => c.ExecutorId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Incident>()
            .ToTable("incidents")
            .HasMany(i => i.Images)
            .WithOne(i => i.Incident)
            .HasForeignKey(i => i.IncidentId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder
            .Entity<IncidentImage>()
            .ToTable("incident_images");
    }
}
