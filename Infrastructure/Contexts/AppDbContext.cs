using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }
    
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        // Configuration des tables
        // --- Users
        builder.Entity<User>(entity => {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();
            
            entity.Property(e => e.Nom)
                .HasColumnName("Nom")
                .HasMaxLength(255)
                .IsRequired();
            
            entity.Property(e => e.Prenom)
                .HasColumnName("Prenom")
                .HasMaxLength(255)
                .IsRequired();
        });
    }
}