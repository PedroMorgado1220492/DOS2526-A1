using Microsoft.EntityFrameworkCore;
using ProductsAPI.Models;

namespace ProductsAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<SalesModel> Sales => Set<SalesModel>();
        public DbSet<SupplierModel> Suppliers => Set<SupplierModel>();
        public DbSet<UsersModel> Users => Set<UsersModel>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=localhost,1433;Database=ProductsDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SalesModel>()
                .HasMany(s => s.Products)
                .WithMany(p => p.Sales)
                .UsingEntity(j => j.ToTable("SalesProducts"));

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesModel>()
                .Property(s => s.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesModel>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sales)
                .HasForeignKey(s => s.UserId);
        }
    }
}