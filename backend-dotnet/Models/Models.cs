namespace ShoppingAssistant.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PreferredCategories { get; set; } // JSON array
    public string? BudgetRange { get; set; } // "low","medium","high"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
    public ICollection<WishlistItem> Wishlist { get; set; } = new List<WishlistItem>();
    public ICollection<CartItem> Cart { get; set; } = new List<CartItem>();
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int Stock { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public string? ImageUrl { get; set; }
    public string? Tags { get; set; } // comma-separated
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ChatSession
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = "New Chat";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}

public class ChatMessage
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public string Role { get; set; } = "user"; // user, assistant
    public string Content { get; set; } = string.Empty;
    public string? ProductIds { get; set; } // JSON array of recommended product IDs
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ChatSession Session { get; set; } = null!;
}

public class WishlistItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public class CartItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
