using CuaHangQuanAo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CuaHangQuanAo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Role
            modelBuilder.Entity<Role>().HasKey(r => r.Id);
            modelBuilder.Entity<Role>().Property(r => r.Name).IsRequired();

            // Configure User
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Username).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.Email).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.PasswordHash).IsRequired();
            modelBuilder.Entity<User>().HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            // Configure Category
            modelBuilder.Entity<Category>().HasKey(c => c.Id);
            modelBuilder.Entity<Category>().Property(c => c.Name).IsRequired();

            // Configure Product
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Product>().Property(p => p.Name).IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.Description).IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>().HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Áo Nam" },
                new Category { Id = 2, Name = "Quần Tây" },
                new Category { Id = 3, Name = "Áo Nữ" },
                new Category { Id = 4, Name = "Quần Jean" },
                new Category { Id = 5, Name = "Phụ Kiện" }
            );

            // Seed Products with sample data
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Áo Sơ mi Trắng Cao Cấp",
                    Description = "Áo sơ mi trắng chất liệu cotton cao cấp, form dáng slimfit, phù hợp cho công sở và các dịp quan trọng.",
                    Price = 250000,
                    CategoryId = 1,
                    ImageUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400&h=600&fit=crop"
                },
                new Product
                {
                    Id = 2,
                    Name = "Quần Âu Slimfit Đen",
                    Description = "Quần âu slimfit màu đen, chất liệu wool pha, tôn dáng và lịch sự cho mọi dịp.",
                    Price = 350000,
                    CategoryId = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1473966968600-fa801b869a1a?w=400&h=600&fit=crop"
                },
                new Product
                {
                    Id = 3,
                    Name = "Áo Thun Basic Trắng",
                    Description = "Áo thun basic màu trắng, chất liệu cotton 100%, thoáng mát và dễ phối đồ.",
                    Price = 120000,
                    CategoryId = 1,
                    ImageUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400&h=600&fit=crop"
                },
                new Product
                {
                    Id = 4,
                    Name = "Quần Jean Xanh Nhạt",
                    Description = "Quần jean xanh nhạt, form dáng straight, chất liệu denim bền đẹp.",
                    Price = 280000,
                    CategoryId = 4,
                    ImageUrl = "https://images.unsplash.com/photo-1542272604-787c3835535d?w=400&h=600&fit=crop"
                },
                new Product
                {
                    Id = 5,
                    Name = "Áo Khoác Da Nâu",
                    Description = "Áo khoác da màu nâu, thiết kế hiện đại, chất liệu da tổng hợp cao cấp.",
                    Price = 450000,
                    CategoryId = 5,
                    ImageUrl = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400&h=600&fit=crop"
                },
                new Product
                {
                    Id = 6,
                    Name = "Áo Len Cổ Lọ",
                    Description = "Áo len cổ lọ màu xám, chất liệu len acrylic ấm áp, phù hợp mùa đông.",
                    Price = 180000,
                    CategoryId = 3,
                    ImageUrl = "https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=400&h=600&fit=crop"
                },
                new Product
                {
                    Id = 7,
                    Name = "Quần Tây Xám",
                    Description = "Quần âu màu xám, form dáng regular, chất liệu wool pha cao cấp.",
                    Price = 320000,
                    CategoryId = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1473966968600-fa801b869a1a?w=400&h=600&fit=crop"
                },
                new Product
                {
                    Id = 8,
                    Name = "Ví Da Nam Đen",
                    Description = "Ví da nam màu đen, thiết kế đơn giản, chứa 8 thẻ và tiền xu.",
                    Price = 150000,
                    CategoryId = 5,
                    ImageUrl = "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=400&h=600&fit=crop"
                }
            );
        }
    }
}
