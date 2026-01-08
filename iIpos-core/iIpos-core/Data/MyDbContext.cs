using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace iIpos_core.Data
{
    public class MyDbContext : DbContext
    {
      
        public MyDbContext(DbContextOptions options) : base(options){}
        #region Dbset
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<TableInfo> TableInfos { get; set; } = null!;
        public DbSet<Store> Stores { get; set; } = null!;
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Feedback> Feedbacks { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Store)
                .WithMany()
                .HasForeignKey(p => p.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Store)
                .WithMany()
                .HasForeignKey(b => b.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Branch)
                .WithMany()
                .HasForeignKey(o => o.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        #endregion

    }
}
