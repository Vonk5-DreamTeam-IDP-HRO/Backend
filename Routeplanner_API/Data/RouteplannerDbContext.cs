using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Models;
using Route = Routeplanner_API.Models.Route;

namespace Routeplanner_API.Data;

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

    public virtual DbSet<UserRight> UserRights { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("locations_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany(p => p.Locations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("locations_user_id_fkey");
        });

        modelBuilder.Entity<LocationDetail>(entity =>
        {
            entity.HasKey(e => e.LocationDetailsId).HasName("location_details_pkey");

            entity.Property(e => e.City).HasDefaultValueSql("'Rotterdam'::character varying");
            entity.Property(e => e.Country).HasDefaultValueSql("'Netherlands'::character varying");

            entity.HasOne(d => d.Location).WithOne(p => p.LocationDetail).HasConstraintName("location_details_location_id_fkey");
        });

        modelBuilder.Entity<LocationRoute>(entity =>
        {
            entity.HasKey(e => e.LocationRouteId).HasName("location_route_pkey");

            entity.HasOne(d => d.Location).WithMany(p => p.LocationRoutes).HasConstraintName("location_route_location_id_fkey");

            entity.HasOne(d => d.Route).WithMany(p => p.LocationRoutes).HasConstraintName("location_route_route_id_fkey");
        });

        modelBuilder.Entity<OpeningTime>(entity =>
        {
            entity.HasKey(e => e.OpeningId).HasName("opening_times_pkey");

            entity.Property(e => e.Is24Hours).HasDefaultValue(false);
            entity.Property(e => e.Timezone).HasDefaultValueSql("'CEST'::character varying");

            entity.HasOne(d => d.Location).WithMany(p => p.OpeningTimes).HasConstraintName("opening_times_location_id_fkey");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("routes_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsPrivate).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Routes)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("routes_created_by_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Right).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_right_id_fkey");
        });

        modelBuilder.Entity<UserConfidential>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_confidential_pkey");

            entity.Property(e => e.UserId).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithOne(p => p.UserConfidential).HasConstraintName("user_confidential_user_id_fkey");
        });

        modelBuilder.Entity<UserRight>(entity =>
        {
            entity.HasKey(e => e.RightId).HasName("user_rights_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
