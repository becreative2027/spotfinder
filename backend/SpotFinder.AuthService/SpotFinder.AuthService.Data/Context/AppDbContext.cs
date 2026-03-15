using Microsoft.EntityFrameworkCore;
using SpotFinder.AuthService.Data.Entities;

namespace SpotFinder.AuthService.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
    public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();
    public DbSet<UserVisit> UserVisits => Set<UserVisit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("auth");

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(500);
            entity.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(150);
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number").HasMaxLength(20);
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(500);
            entity.Property(e => e.Provider).HasColumnName("provider").HasMaxLength(50).HasDefaultValue("local");
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(50).HasDefaultValue("user");
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<UserRefreshToken>(entity =>
        {
            entity.ToTable("user_refresh_tokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Token).HasColumnName("token").HasMaxLength(500).IsRequired();
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.IsRevoked).HasColumnName("is_revoked").HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.HasOne(e => e.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(e => e.UserId);
        });

        modelBuilder.Entity<UserFavorite>(entity =>
        {
            entity.ToTable("user_favorites");
            entity.HasKey(e => new { e.UserId, e.VenueId });
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VenueId).HasColumnName("venue_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Favorites)
                  .HasForeignKey(e => e.UserId);
        });

        modelBuilder.Entity<UserVisit>(entity =>
        {
            entity.ToTable("user_visits");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VenueId).HasColumnName("venue_id");
            entity.Property(e => e.VisitedAt).HasColumnName("visited_at").HasDefaultValueSql("now()");
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Visits)
                  .HasForeignKey(e => e.UserId);
        });
    }
}
