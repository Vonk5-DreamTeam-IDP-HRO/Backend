using Microsoft.EntityFrameworkCore;

namespace Routeplanner_API.Models;

public partial class RouteplannerDbContext : DbContext
{
    public RouteplannerDbContext()
    {
    }

    public RouteplannerDbContext(DbContextOptions<RouteplannerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<LocationDetail> LocationDetails { get; set; }

    public virtual DbSet<LocationRoute> LocationRoutes { get; set; }

    public virtual DbSet<OpeningTime> OpeningTimes { get; set; }

    public virtual DbSet<Route> Routes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserConfidential> UserConfidentials { get; set; }

    public virtual DbSet<UserPermission> UserRights { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("locations_pkey");

            entity.ToTable("locations");

            entity.Property(e => e.LocationId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("location_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Locations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("locations_user_id_fkey");
        });

        modelBuilder.Entity<LocationDetail>(entity =>
        {
            entity.HasKey(e => e.LocationDetailsId).HasName("location_details_pkey");

            entity.ToTable("location_details");

            entity.HasIndex(e => e.LocationId, "location_details_location_id_key").IsUnique();

            entity.Property(e => e.LocationDetailsId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("location_details_id");
            entity.Property(e => e.Accessibility)
                .HasMaxLength(255)
                .HasColumnName("accessibility");
            entity.Property(e => e.Address)
                .HasMaxLength(180)
                .HasColumnName("address");
            entity.Property(e => e.Category)
                .HasMaxLength(60)
                .HasColumnName("category");
            entity.Property(e => e.City)
                .HasMaxLength(120)
                .HasDefaultValueSql("'Rotterdam'::character varying")
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(60)
                .HasDefaultValueSql("'Netherlands'::character varying")
                .HasColumnName("country");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(18)
                .HasColumnName("phone_number");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .HasColumnName("website");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(6)
                .HasColumnName("zip_code");

            entity.HasOne(d => d.Location).WithOne(p => p.LocationDetail)
                .HasForeignKey<LocationDetail>(d => d.LocationId)
                .HasConstraintName("location_details_location_id_fkey");
        });

        modelBuilder.Entity<LocationRoute>(entity =>
        {
            entity.HasKey(e => e.LocationRouteId).HasName("location_route_pkey");

            entity.ToTable("location_route");

            entity.HasIndex(e => new { e.RouteId, e.LocationId }, "location_route_route_id_location_id_key").IsUnique();

            entity.Property(e => e.LocationRouteId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("location_route_id");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.RouteId).HasColumnName("route_id");

            entity.HasOne(d => d.Location).WithMany(p => p.LocationRoutes)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("location_route_location_id_fkey");

            entity.HasOne(d => d.Route).WithMany(p => p.LocationRoutes)
                .HasForeignKey(d => d.RouteId)
                .HasConstraintName("location_route_route_id_fkey");
        });

        modelBuilder.Entity<OpeningTime>(entity =>
        {
            entity.HasKey(e => e.OpeningId).HasName("opening_times_pkey");

            entity.ToTable("opening_times");

            entity.Property(e => e.OpeningId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("opening_id");
            entity.Property(e => e.CloseTime).HasColumnName("close_time");
            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(10)
                .HasColumnName("day_of_week");
            entity.Property(e => e.Is24Hours)
                .HasDefaultValue(false)
                .HasColumnName("is_24_hours");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.OpenTime).HasColumnName("open_time");
            entity.Property(e => e.Timezone)
                .HasMaxLength(10)
                .HasDefaultValueSql("'CEST'::character varying")
                .HasColumnName("timezone");

            entity.HasOne(d => d.Location).WithMany(p => p.OpeningTimes)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("opening_times_location_id_fkey");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("routes_pkey");

            entity.ToTable("routes");

            entity.HasIndex(e => e.IsPrivate, "idx_routes_is_private");

            entity.Property(e => e.RouteId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("route_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.IsPrivate)
                .HasDefaultValue(false)
                .HasColumnName("is_private");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Routes)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("routes_created_by_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users", "auth");

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(72)
                .HasColumnName("password_hash");
            entity.Property(e => e.UserRightId).HasColumnName("right_id");
            entity.Property(e => e.Username)
                .HasMaxLength(25)
                .HasColumnName("username");

            entity.HasOne(d => d.Right).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_right_id_fkey");
        });

        modelBuilder.Entity<UserConfidential>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_confidential_pkey");

            entity.ToTable("user_confidential", "auth");

            entity.HasIndex(e => e.Email, "user_confidential_email_key").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");

            entity.HasOne(d => d.User).WithOne(p => p.UserConfidential)
                .HasForeignKey<UserConfidential>(d => d.UserId)
                .HasConstraintName("user_confidential_user_id_fkey");
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(e => e.UserRightId).HasName("user_rights_pkey");

            entity.ToTable("user_rights", "auth");

            entity.HasIndex(e => e.UserRightName, "user_rights_right_name_key").IsUnique();

            entity.Property(e => e.UserRightId).HasColumnName("right_id");
            entity.Property(e => e.UserRightName)
                .HasMaxLength(20)
                .HasColumnName("right_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
