using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TomNam.Models;

namespace TomNam.Data
{
    public class DataContext : IdentityDbContext<User>
    //should use custom Identity User
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        public DbSet<CustomerProfile> CustomerProfile { get; set; }
        public DbSet<OwnerProfile> OwnerProfile { get; set; }
        public DbSet<Karenderya> Karenderya { get; set; }
        public DbSet<ProofOfBusiness> ProofOfBusiness { get; set; }
        public DbSet<Food> Food { get; set; }
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<ReservedItem> ReservedItem { get; set; }
        public DbSet<Transaction> Transaction { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // modelBuilder.Entity<User>()
            // .if ang user kay role niya is Customer naa syay Customer Profile butngan related name nga Profile
            // .if ang user kay role niya is Owner naa syay Owner Profile butngan related name nga Profile


            modelBuilder
                .Entity<Karenderya>()
                .HasOne(k => k.User) // Karenderya has one User
                .WithOne() // User has one Karenderya (no navigation property on User)
                .HasForeignKey<Karenderya>(k => k.UserId) // Foreign key in Karenderya
                .IsRequired() // The relationship is required
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

            modelBuilder
                .Entity<Karenderya>()
                .HasIndex(k => k.UserId) // Make sure the foreign key is unique (1-to-1 relationship)
                .IsUnique();

            modelBuilder.Entity<Karenderya>().Property(p => p.IsVerified).HasDefaultValue(false); // Sets the isVerified property to false by default

            modelBuilder
                .Entity<Karenderya>() // Sets the karenderya rating to 0
                .Property(p => p.Rating)
                .HasPrecision(1, 2)
                .HasDefaultValue(0.00);

            modelBuilder.Entity<OwnerProfile>()
                .HasOne(op => op.Karenderya)
                .WithOne()
                .HasForeignKey<OwnerProfile>(op => op.KarenderyaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<ProofOfBusiness>()
                .HasOne(p => p.Karenderya)
                .WithOne(k => k.proofOfBusiness)
                .HasForeignKey<ProofOfBusiness>(p => p.KarenderyaId); // ProofOfBusiness has one User

            modelBuilder.Entity<CartItem>().Property(p => p.IsChecked).HasDefaultValue(true);

            modelBuilder.Entity<Reservation>()
               .HasOne(r => r.Customer)
               .WithMany()
               .HasForeignKey(r => r.CustomerProfileId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Karenderya)
                .WithMany()
                .HasForeignKey(r => r.KarenderyaId);

            modelBuilder.Entity<ReservedItem>()
               .HasOne(ri => ri.Reservation)
               .WithMany(r => r.ReservedItems)
               .HasForeignKey(ri => ri.ReservationId)
               .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ReservedItem>()
                .HasOne(ri => ri.Food)
                .WithMany()
                .HasForeignKey(ri => ri.FoodId)
                .OnDelete(DeleteBehavior.Restrict);

            // modelBuilder.Entity<Transaction>()
            //     .HasOne(t => t.Reservation)
            //     .WithMany() soyjack
            //     .HasForeignKey(t => t.FoodId)
            //     .OnDelete(DeleteBehavior.Restrict); soyjack ni

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Reservation)
                .WithOne() // 1:1 relationship
                .HasForeignKey<Transaction>(t => t.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
