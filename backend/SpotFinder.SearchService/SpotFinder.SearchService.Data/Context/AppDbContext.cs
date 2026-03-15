using Microsoft.EntityFrameworkCore;
using SpotFinder.SearchService.Data.Entities;

namespace SpotFinder.SearchService.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<VenueReadModel> Venues => Set<VenueReadModel>();
    public DbSet<DistrictReadModel> Districts => Set<DistrictReadModel>();
    public DbSet<ConceptTagReadModel> ConceptTags => Set<ConceptTagReadModel>();
    public DbSet<VenuePhotoReadModel> VenuePhotos => Set<VenuePhotoReadModel>();
    public DbSet<VenueConceptReadModel> VenueConcepts => Set<VenueConceptReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Map to venue schema tables — column names match VenueService migrations (PascalCase, no HasColumnName overrides)
        modelBuilder.Entity<VenueReadModel>(e =>
        {
            e.ToTable("venues", "venue");
            e.HasKey(v => v.Id);
            e.Property(v => v.Lat).HasPrecision(10, 8);
            e.Property(v => v.Lng).HasPrecision(11, 8);
            e.Property(v => v.AverageRating).HasPrecision(3, 2);

            e.HasOne(v => v.District)
                .WithMany()
                .HasForeignKey(v => v.DistrictId);

            e.HasMany(v => v.Photos)
                .WithOne()
                .HasForeignKey(p => p.VenueId);

            e.HasMany(v => v.VenueConcepts)
                .WithOne()
                .HasForeignKey(vc => vc.VenueId);
        });

        modelBuilder.Entity<DistrictReadModel>(e =>
        {
            e.ToTable("districts", "venue");
            e.HasKey(d => d.Id);
        });

        modelBuilder.Entity<ConceptTagReadModel>(e =>
        {
            e.ToTable("concept_tags", "venue");
            e.HasKey(ct => ct.Id);
        });

        modelBuilder.Entity<VenuePhotoReadModel>(e =>
        {
            e.ToTable("venue_photos", "venue");
            e.HasKey(p => p.Id);
        });

        modelBuilder.Entity<VenueConceptReadModel>(e =>
        {
            e.ToTable("venue_concepts", "venue");
            e.HasKey(vc => new { vc.VenueId, vc.ConceptTagId });

            e.HasOne(vc => vc.ConceptTag)
                .WithMany()
                .HasForeignKey(vc => vc.ConceptTagId);
        });
    }
}
