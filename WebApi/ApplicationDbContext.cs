using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Shoe> Shoes { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Client> Clients { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shoe>()
                 .HasOne(s => s.Brand)
                 .WithMany(b => b.Shoes)
                 .HasForeignKey(s => s.BrandId);

            modelBuilder.Entity<Shoe>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Shoes)
                .HasForeignKey(s => s.CategoryId);

            modelBuilder.Entity<Brand>()
                .HasIndex(b => b.Name)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasOne(s => s.Role)
                .WithMany(s => s.Clients)
                .HasForeignKey(s => s.RoleId);

            modelBuilder.Entity<Role>()
                .HasIndex(b => b.Name)
                .IsUnique();
        }
    }
}
