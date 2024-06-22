using cw9.Models;
using Microsoft.EntityFrameworkCore;

namespace cw9.Data;

public class DatabaseContext : DbContext
{
    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }
    
    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientTrip> ClientTrips { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Client>(e =>
        {
            e.HasKey(e => e.IdClient);

            e.ToTable("Client");

            e.Property(e => e.Email)
                .HasMaxLength(120);
            e.Property(e => e.Firstname)
                .HasMaxLength(120);
            e.Property(e => e.Lastname)
                .HasMaxLength(120);
            e.Property(e => e.Pesel)
                .HasMaxLength(120);
            e.Property(e => e.Telephone)
                .HasMaxLength(120);
        });

        modelBuilder.Entity<ClientTrip>(entity =>
        {
            entity.HasKey(e => new { e.IdClient, e.IdTrip });

            entity.ToTable("Client_Trip");

            entity.Property(e => e.PaymentDate)
                .IsRequired(false);

            entity.HasOne(d => d.Client)
                .WithMany(p => p.ClientTrips)
                .HasForeignKey(d => d.IdClient);

            entity.HasOne(d => d.Trip)
                .WithMany(p => p.ClientTrips)
                .HasForeignKey(d => d.IdTrip);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.IdCountry);

            entity.ToTable("Country");

            entity.Property(e => e.Name)
                .HasMaxLength(120);

            entity.HasMany(d => d.Trips).WithMany(p => p.Countries)
                .UsingEntity<Dictionary<string, object>>(
                    "CountryTrip",
                    r => r.HasOne<Trip>().WithMany()
                        .HasForeignKey("IdTrip"),
                    l => l.HasOne<Country>().WithMany()
                        .HasForeignKey("IdCountry"),
                    j =>
                    {
                        j.HasKey("IdCountry", "IdTrip");
                        j.ToTable("Country_Trip");
                        j.IndexerProperty<int>("IdCountry");
                        j.IndexerProperty<int>("IdTrip");
                    });
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.IdTrip);

            entity.ToTable("Trip");

            entity.Property(e => e.Description)
                .HasMaxLength(220);
            entity.Property(e => e.Name)
                .HasMaxLength(120);
        });
    }
}