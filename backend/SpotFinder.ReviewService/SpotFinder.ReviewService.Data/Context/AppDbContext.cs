using Microsoft.EntityFrameworkCore;
using SpotFinder.ReviewService.Data.Entities;

namespace SpotFinder.ReviewService.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Review> Reviews => Set<Review>();

    // Write model for venue rating updates — maps to venue.venues (same shared DB)
    public DbSet<VenueRating> VenueRatings => Set<VenueRating>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Review>(e =>
        {
            e.ToTable("reviews", "review");
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(r => r.Body).HasColumnType("text");
            e.Property(r => r.Rating).HasColumnType("smallint");
            e.Property(r => r.Status).HasMaxLength(20).HasDefaultValue("pending");
            e.Property(r => r.CreatedAt).HasDefaultValueSql("now()");
        });

        // Maps to venue.venues — only AverageRating and ReviewCount fields
        modelBuilder.Entity<VenueRating>(e =>
        {
            e.ToTable("venues", "venue");
            e.HasKey(v => v.Id);
            e.Property(v => v.AverageRating).HasPrecision(3, 2);
        });
    }
}
