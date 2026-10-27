namespace ShoppingAssistant.DTOs;

// Auth
public record RegisterDto(string Name, string Email, string Password);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(string Token, string Name, string Email);

// Product
public record ProductDto(int Id, string Name, string Description, string Category, string Brand,
    decimal Price, decimal? OriginalPrice, int Stock, double Rating, int ReviewCount,
    string? ImageUrl, string? Tags, bool IsActive);
public record CreateProductDto(string Name, string Description, string Category, string Brand,
    decimal Price, decimal? OriginalPrice, int Stock, double Rating, int ReviewCount,
    string? ImageUrl, string? Tags);

// Chat
public record StartChatDto(string? InitialMessage);
public record SendMessageDto(string Message);
public record ChatMessageDto(int Id, string Role, string Content, List<ProductDto>? RecommendedProducts, DateTime CreatedAt);
public record ChatSessionDto(int Id, string Title, DateTime CreatedAt, DateTime UpdatedAt, List<ChatMessageDto> Messages);
public record ChatSessionSummaryDto(int Id, string Title, DateTime CreatedAt, DateTime UpdatedAt, int MessageCount);

// AI Request/Response
public record AiChatRequest(string Message, List<AiMessage> History, UserPreferencesDto? Preferences, List<ProductDto> Products);
public record AiMessage(string Role, string Content);
public record AiChatResponse(string Reply, List<int> RecommendedProductIds, string? Intent, List<string> Suggestions);
public record UserPreferencesDto(string? BudgetRange, List<string>? PreferredCategories);

// Wishlist & Cart
public record AddToWishlistDto(int ProductId);
public record AddToCartDto(int ProductId, int Quantity);
public record CartItemDto(int Id, int ProductId, string ProductName, string Brand, decimal Price, int Quantity, decimal LineTotal, string? ImageUrl);
public record CartSummaryDto(List<CartItemDto> Items, decimal Total, int ItemCount);
public record WishlistItemDto(int Id, int ProductId, string ProductName, string Brand, decimal Price, double Rating, string? ImageUrl, DateTime AddedAt);

// Preferences
public record UpdatePreferencesDto(string? BudgetRange, List<string>? PreferredCategories);

// Pagination
public record PagedResult<T>(List<T> Data, int TotalCount, int Page, int PageSize, int TotalPages);
