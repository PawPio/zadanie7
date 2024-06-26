﻿using Microsoft.EntityFrameworkCore;
using Zadanie7.Models;

namespace Zadanie7.Contexts;

public partial class Context : DbContext
{
    public Context()
    {
    }

    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }
    
    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientTrip> ClientTrips { get; set; }

    public virtual DbSet<Country> Countries { get; set; }
    
    public virtual DbSet<Trip> Trips { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.
        UseMySQL("Server=localhost;User ID=user;Password=paswword;Database=todo0;Port=3306;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.IdClient).HasName("PRIMARY");

            entity.ToTable("Client");

            entity.HasIndex(e => e.Pesel, "Pesel").IsUnique();

            entity.Property(e => e.Email);
            entity.Property(e => e.FirstName);
            entity.Property(e => e.LastName);
            entity.Property(e => e.Pesel).HasMaxLength(11);
            entity.Property(e => e.Telephone);
        });

        modelBuilder.Entity<ClientTrip>(entity =>
        {
            entity.HasKey(e => new { e.IdClient, e.IdTrip }).HasName("PRIMARY");

            entity.ToTable("Client_Trip");

            entity.HasIndex(e => e.IdTrip, "IdTrip");

            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.RegisteredAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.ClientTrips)
                .HasForeignKey(d => d.IdClient)
                .HasConstraintName("Client_Trip_ibfk_1");

            entity.HasOne(d => d.IdTripNavigation).WithMany(p => p.ClientTrips)
                .HasForeignKey(d => d.IdTrip)
                .HasConstraintName("Client_Trip_ibfk_2");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.IdCountry).HasName("PRIMARY");

            entity.ToTable("Country");

            entity.Property(e => e.Name);

            entity.HasMany(d => d.IdTrips).WithMany(p => p.IdCountries)
                .UsingEntity<Dictionary<string, object>>(
                    "CountryTrip",
                    r => r.HasOne<Trip>().WithMany()
                        .HasForeignKey("IdTrip")
                        .HasConstraintName("Country_Trip_ibfk_2"),
                    l => l.HasOne<Country>().WithMany()
                        .HasForeignKey("IdCountry")
                        .HasConstraintName("Country_Trip_ibfk_1"),
                    j =>
                    {
                        j.HasKey("IdCountry", "IdTrip").HasName("PRIMARY");
                        j.ToTable("Country_Trip");
                        j.HasIndex(new[] { "IdTrip" }, "IdTrip");
                    });
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.IdTrip).HasName("PRIMARY");

            entity.ToTable("Trip");

            entity.Property(e => e.DateFrom).HasColumnType("datetime");
            entity.Property(e => e.DateTo).HasColumnType("datetime");
            entity.Property(e => e.Description);
            entity.Property(e => e.Name);
        });
        CreatingPartial(modelBuilder);
    }

    partial void CreatingPartial(ModelBuilder modelBuilder);
}