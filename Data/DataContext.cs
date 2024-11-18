using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TomNam.Models;

namespace TomNam.Data
{
    public class DataContext : IdentityDbContext<User>
        //should use custom Identity User
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { 

        }

        public DbSet<Karenderya> Karenderya { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Karenderya>()
                .HasOne(k => k.User)  // Karenderya has one User
                .WithOne()  // User has one Karenderya (no navigation property on User)
                .HasForeignKey<Karenderya>(k => k.UserId)  // Foreign key in Karenderya
                .IsRequired()  // The relationship is required
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading deletes
            
            modelBuilder.Entity<Karenderya>()
                .HasIndex(k => k.UserId)  // Make sure the foreign key is unique (1-to-1 relationship)
                .IsUnique();

            modelBuilder.Entity<Karenderya>()
               .Property(p => p.Rating)
               .HasPrecision(1, 2)
               .HasDefaultValue(0.00); 
        }
    }
}