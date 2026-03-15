using Microsoft.EntityFrameworkCore;
using SpotFinder.VenueService.Data.Entities;

namespace SpotFinder.VenueService.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<District> Districts => Set<District>();
    public DbSet<ConceptTag> ConceptTags => Set<ConceptTag>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<VenuePhoto> VenuePhotos => Set<VenuePhoto>();
    public DbSet<VenueConcept> VenueConcepts => Set<VenueConcept>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // District
        modelBuilder.Entity<District>(e =>
        {
            e.ToTable("districts", "venue");
            e.HasKey(d => d.Id);
            e.Property(d => d.Id).UseIdentityColumn();
            e.Property(d => d.Name).HasMaxLength(100).IsRequired();
            e.Property(d => d.City).HasMaxLength(100).HasDefaultValue("İstanbul");
        });

        // ConceptTag
        modelBuilder.Entity<ConceptTag>(e =>
        {
            e.ToTable("concept_tags", "venue");
            e.HasKey(ct => ct.Id);
            e.Property(ct => ct.Id).UseIdentityColumn();
            e.Property(ct => ct.NameTr).HasMaxLength(100).IsRequired();
            e.Property(ct => ct.NameEn).HasMaxLength(100).IsRequired();
            e.Property(ct => ct.IsSystem).HasDefaultValue(false);
            e.Property(ct => ct.IsActive).HasDefaultValue(true);
            e.Property(ct => ct.CreatedAt).HasDefaultValueSql("now()");
        });

        // Venue
        modelBuilder.Entity<Venue>(e =>
        {
            e.ToTable("venues", "venue");
            e.HasKey(v => v.Id);
            e.Property(v => v.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(v => v.Name).HasMaxLength(200).IsRequired();
            e.Property(v => v.Description).HasColumnType("text");
            e.Property(v => v.Address).HasMaxLength(500);
            e.Property(v => v.ParkingStatus).HasMaxLength(20);
            e.Property(v => v.Lat).HasColumnType("decimal(10,8)");
            e.Property(v => v.Lng).HasColumnType("decimal(11,8)");
            e.Property(v => v.AverageRating).HasColumnType("decimal(3,2)").HasDefaultValue(0m);
            e.Property(v => v.ReviewCount).HasDefaultValue(0);
            e.Property(v => v.IsActive).HasDefaultValue(true);
            e.Property(v => v.CreatedAt).HasDefaultValueSql("now()");
            e.HasOne(v => v.District).WithMany(d => d.Venues).HasForeignKey(v => v.DistrictId);
        });

        // VenuePhoto
        modelBuilder.Entity<VenuePhoto>(e =>
        {
            e.ToTable("venue_photos", "venue");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(p => p.Url).HasMaxLength(500).IsRequired();
            e.Property(p => p.IsMenuPhoto).HasDefaultValue(false);
            e.Property(p => p.DisplayOrder).HasDefaultValue(0);
            e.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
            e.HasOne(p => p.Venue).WithMany(v => v.Photos).HasForeignKey(p => p.VenueId);
        });

        // VenueConcept (composite PK)
        modelBuilder.Entity<VenueConcept>(e =>
        {
            e.ToTable("venue_concepts", "venue");
            e.HasKey(vc => new { vc.VenueId, vc.ConceptTagId });
            e.HasOne(vc => vc.Venue).WithMany(v => v.VenueConcepts).HasForeignKey(vc => vc.VenueId);
            e.HasOne(vc => vc.ConceptTag).WithMany(ct => ct.VenueConcepts).HasForeignKey(vc => vc.ConceptTagId);
        });

        // Seed: 39 Istanbul districts
        modelBuilder.Entity<District>().HasData(
            new District { Id = 1, Name = "Adalar", City = "İstanbul" },
            new District { Id = 2, Name = "Arnavutköy", City = "İstanbul" },
            new District { Id = 3, Name = "Ataşehir", City = "İstanbul" },
            new District { Id = 4, Name = "Avcılar", City = "İstanbul" },
            new District { Id = 5, Name = "Bağcılar", City = "İstanbul" },
            new District { Id = 6, Name = "Bahçelievler", City = "İstanbul" },
            new District { Id = 7, Name = "Bakırköy", City = "İstanbul" },
            new District { Id = 8, Name = "Başakşehir", City = "İstanbul" },
            new District { Id = 9, Name = "Bayrampaşa", City = "İstanbul" },
            new District { Id = 10, Name = "Beşiktaş", City = "İstanbul" },
            new District { Id = 11, Name = "Beykoz", City = "İstanbul" },
            new District { Id = 12, Name = "Beylikdüzü", City = "İstanbul" },
            new District { Id = 13, Name = "Beyoğlu", City = "İstanbul" },
            new District { Id = 14, Name = "Büyükçekmece", City = "İstanbul" },
            new District { Id = 15, Name = "Çatalca", City = "İstanbul" },
            new District { Id = 16, Name = "Çekmeköy", City = "İstanbul" },
            new District { Id = 17, Name = "Esenler", City = "İstanbul" },
            new District { Id = 18, Name = "Esenyurt", City = "İstanbul" },
            new District { Id = 19, Name = "Eyüpsultan", City = "İstanbul" },
            new District { Id = 20, Name = "Fatih", City = "İstanbul" },
            new District { Id = 21, Name = "Gaziosmanpaşa", City = "İstanbul" },
            new District { Id = 22, Name = "Güngören", City = "İstanbul" },
            new District { Id = 23, Name = "Kadıköy", City = "İstanbul" },
            new District { Id = 24, Name = "Kağıthane", City = "İstanbul" },
            new District { Id = 25, Name = "Kartal", City = "İstanbul" },
            new District { Id = 26, Name = "Küçükçekmece", City = "İstanbul" },
            new District { Id = 27, Name = "Maltepe", City = "İstanbul" },
            new District { Id = 28, Name = "Pendik", City = "İstanbul" },
            new District { Id = 29, Name = "Sancaktepe", City = "İstanbul" },
            new District { Id = 30, Name = "Sarıyer", City = "İstanbul" },
            new District { Id = 31, Name = "Silivri", City = "İstanbul" },
            new District { Id = 32, Name = "Sultanbeyli", City = "İstanbul" },
            new District { Id = 33, Name = "Sultangazi", City = "İstanbul" },
            new District { Id = 34, Name = "Şile", City = "İstanbul" },
            new District { Id = 35, Name = "Şişli", City = "İstanbul" },
            new District { Id = 36, Name = "Tuzla", City = "İstanbul" },
            new District { Id = 37, Name = "Ümraniye", City = "İstanbul" },
            new District { Id = 38, Name = "Üsküdar", City = "İstanbul" },
            new District { Id = 39, Name = "Zeytinburnu", City = "İstanbul" }
        );

        // Seed: 25 system concept tags
        modelBuilder.Entity<ConceptTag>().HasData(
            new ConceptTag { Id = 1, NameTr = "Doğum Günü", NameEn = "Birthday", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 2, NameTr = "Romantik", NameEn = "Romantic", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 3, NameTr = "Manzaralı", NameEn = "Scenic View", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 4, NameTr = "Açık Hava", NameEn = "Outdoor", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 5, NameTr = "Kahvaltı", NameEn = "Breakfast", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 6, NameTr = "Canlı Müzik", NameEn = "Live Music", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 7, NameTr = "Aile Dostu", NameEn = "Family Friendly", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 8, NameTr = "İş Yemeği", NameEn = "Business Lunch", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 9, NameTr = "Vegan / Vejetaryen", NameEn = "Vegan / Vegetarian", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 10, NameTr = "Deniz Kenarı", NameEn = "Waterfront", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 11, NameTr = "Kahve & Tatlı", NameEn = "Coffee & Dessert", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 12, NameTr = "Gece Hayatı", NameEn = "Nightlife", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 13, NameTr = "Brunch", NameEn = "Brunch", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 14, NameTr = "Fast Food", NameEn = "Fast Food", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 15, NameTr = "Fine Dining", NameEn = "Fine Dining", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 16, NameTr = "Tarihî Mekân", NameEn = "Historic Venue", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 17, NameTr = "Köpek Dostu", NameEn = "Pet Friendly", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 18, NameTr = "Çay Bahçesi", NameEn = "Tea Garden", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 19, NameTr = "Spor Etkinliği", NameEn = "Sports Event", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 20, NameTr = "Sanat & Kültür", NameEn = "Art & Culture", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 21, NameTr = "Boğaz Manzarası", NameEn = "Bosphorus View", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 22, NameTr = "Nargile", NameEn = "Hookah", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 23, NameTr = "Oyun Alanı", NameEn = "Playground", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 24, NameTr = "Rooftop", NameEn = "Rooftop", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ConceptTag { Id = 25, NameTr = "Çalışma Dostu", NameEn = "Work Friendly", IsSystem = true, IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
