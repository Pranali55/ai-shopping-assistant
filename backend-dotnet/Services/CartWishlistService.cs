using Microsoft.EntityFrameworkCore;
using ShoppingAssistant.Data;
using ShoppingAssistant.DTOs;
using ShoppingAssistant.Models;

namespace ShoppingAssistant.Services;

public class CartWishlistService
{
    private readonly AppDbContext _db;
    public CartWishlistService(AppDbContext db) => _db = db;

    // Cart
    public async Task<CartSummaryDto> GetCartAsync(int userId)
    {
        var items = await _db.CartItems.Include(c => c.Product).Where(c => c.UserId == userId).ToListAsync();
        var dtos = items.Select(c => new CartItemDto(c.Id, c.ProductId, c.Product.Name, c.Product.Brand,
            c.Product.Price, c.Quantity, c.Quantity * c.Product.Price, c.Product.ImageUrl)).ToList();
        return new CartSummaryDto(dtos, dtos.Sum(i => i.LineTotal), dtos.Sum(i => i.Quantity));
    }

    public async Task<CartSummaryDto> AddToCartAsync(int userId, AddToCartDto dto)
    {
        var existing = await _db.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);
        if (existing != null)
            existing.Quantity += dto.Quantity;
        else
            _db.CartItems.Add(new CartItem { UserId = userId, ProductId = dto.ProductId, Quantity = dto.Quantity });
        await _db.SaveChangesAsync();
        return await GetCartAsync(userId);
    }

    public async Task<bool> RemoveFromCartAsync(int userId, int cartItemId)
    {
        var item = await _db.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);
        if (item == null) return false;
        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task ClearCartAsync(int userId)
    {
        var items = _db.CartItems.Where(c => c.UserId == userId);
        _db.CartItems.RemoveRange(items);
        await _db.SaveChangesAsync();
    }

    // Wishlist
    public async Task<List<WishlistItemDto>> GetWishlistAsync(int userId)
    {
        return await _db.WishlistItems.Include(w => w.Product)
            .Where(w => w.UserId == userId)
            .Select(w => new WishlistItemDto(w.Id, w.ProductId, w.Product.Name, w.Product.Brand,
                w.Product.Price, w.Product.Rating, w.Product.ImageUrl, w.AddedAt))
            .ToListAsync();
    }

    public async Task<bool> AddToWishlistAsync(int userId, AddToWishlistDto dto)
    {
        if (_db.WishlistItems.Any(w => w.UserId == userId && w.ProductId == dto.ProductId)) return false;
        _db.WishlistItems.Add(new WishlistItem { UserId = userId, ProductId = dto.ProductId });
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveFromWishlistAsync(int userId, int wishlistItemId)
    {
        var item = await _db.WishlistItems.FirstOrDefaultAsync(w => w.Id == wishlistItemId && w.UserId == userId);
        if (item == null) return false;
        _db.WishlistItems.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }
}
