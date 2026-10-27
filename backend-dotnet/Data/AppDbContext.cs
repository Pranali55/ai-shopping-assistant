using Microsoft.EntityFrameworkCore;
using ShoppingAssistant.Models;

namespace ShoppingAssistant.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Product>().Property(p => p.OriginalPrice).HasColumnType("decimal(18,2)");

        // Seed products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "iPhone 15 Pro", Description = "Latest Apple flagship with A17 Pro chip, titanium design", Category = "Electronics", Brand = "Apple", Price = 999.99m, OriginalPrice = 1099.99m, Stock = 50, Rating = 4.8, ReviewCount = 1240, Tags = "smartphone,apple,ios,5g", ImageUrl = "https://via.placeholder.com/300x300?text=iPhone+15+Pro" },
            new Product { Id = 2, Name = "Samsung Galaxy S24", Description = "Android flagship with AI features and 200MP camera", Category = "Electronics", Brand = "Samsung", Price = 849.99m, OriginalPrice = 899.99m, Stock = 75, Rating = 4.7, ReviewCount = 980, Tags = "smartphone,android,samsung,5g", ImageUrl = "https://via.placeholder.com/300x300?text=Galaxy+S24" },
            new Product { Id = 3, Name = "MacBook Pro 14\"", Description = "M3 Pro chip, 18GB RAM, perfect for developers", Category = "Laptops", Brand = "Apple", Price = 1999.99m, OriginalPrice = 2199.99m, Stock = 30, Rating = 4.9, ReviewCount = 560, Tags = "laptop,apple,macbook,developer", ImageUrl = "https://via.placeholder.com/300x300?text=MacBook+Pro" },
            new Product { Id = 4, Name = "Sony WH-1000XM5", Description = "Industry-leading noise cancelling headphones", Category = "Audio", Brand = "Sony", Price = 299.99m, OriginalPrice = 349.99m, Stock = 100, Rating = 4.8, ReviewCount = 2100, Tags = "headphones,wireless,noise-cancelling,sony", ImageUrl = "https://via.placeholder.com/300x300?text=Sony+WH1000XM5" },
            new Product { Id = 5, Name = "Nike Air Max 270", Description = "Comfortable running shoes with Air cushioning", Category = "Footwear", Brand = "Nike", Price = 129.99m, OriginalPrice = 159.99m, Stock = 200, Rating = 4.5, ReviewCount = 3400, Tags = "shoes,running,nike,sports", ImageUrl = "https://via.placeholder.com/300x300?text=Nike+Air+Max" },
            new Product { Id = 6, Name = "Kindle Paperwhite", Description = "Waterproof e-reader with 6.8\" display", Category = "Electronics", Brand = "Amazon", Price = 139.99m, OriginalPrice = 159.99m, Stock = 80, Rating = 4.6, ReviewCount = 5600, Tags = "ereader,kindle,books,amazon", ImageUrl = "https://via.placeholder.com/300x300?text=Kindle+Paperwhite" },
            new Product { Id = 7, Name = "Dell XPS 15", Description = "OLED display, Intel i9, 32GB RAM powerhouse laptop", Category = "Laptops", Brand = "Dell", Price = 1799.99m, OriginalPrice = 1999.99m, Stock = 25, Rating = 4.6, ReviewCount = 430, Tags = "laptop,dell,windows,oled", ImageUrl = "https://via.placeholder.com/300x300?text=Dell+XPS+15" },
            new Product { Id = 8, Name = "Apple Watch Series 9", Description = "Advanced health tracking with Double Tap gesture", Category = "Wearables", Brand = "Apple", Price = 399.99m, OriginalPrice = 429.99m, Stock = 60, Rating = 4.7, ReviewCount = 870, Tags = "smartwatch,apple,health,fitness", ImageUrl = "https://via.placeholder.com/300x300?text=Apple+Watch+S9" },
            new Product { Id = 9, Name = "Levi's 501 Original Jeans", Description = "Classic straight fit jeans, timeless style", Category = "Clothing", Brand = "Levi's", Price = 69.99m, OriginalPrice = 89.99m, Stock = 300, Rating = 4.4, ReviewCount = 7800, Tags = "jeans,denim,levis,fashion", ImageUrl = "https://via.placeholder.com/300x300?text=Levis+501" },
            new Product { Id = 10, Name = "Instant Pot Duo 7-in-1", Description = "Multi-use pressure cooker, slow cooker, rice cooker", Category = "Kitchen", Brand = "Instant Pot", Price = 89.99m, OriginalPrice = 119.99m, Stock = 150, Rating = 4.7, ReviewCount = 12000, Tags = "kitchen,cooking,instantpot,appliance", ImageUrl = "https://via.placeholder.com/300x300?text=Instant+Pot" },
            new Product { Id = 11, Name = "iPad Air M2", Description = "Lightweight tablet with M2 chip and 10.9\" display", Category = "Electronics", Brand = "Apple", Price = 599.99m, OriginalPrice = 649.99m, Stock = 45, Rating = 4.8, ReviewCount = 1100, Tags = "tablet,apple,ipad,m2", ImageUrl = "https://via.placeholder.com/300x300?text=iPad+Air" },
            new Product { Id = 12, Name = "Adidas Ultraboost 23", Description = "Responsive running shoes with Boost midsole", Category = "Footwear", Brand = "Adidas", Price = 179.99m, OriginalPrice = 199.99m, Stock = 180, Rating = 4.6, ReviewCount = 2300, Tags = "shoes,running,adidas,boost", ImageUrl = "https://via.placeholder.com/300x300?text=Adidas+Ultraboost" }
        );
    }
}
