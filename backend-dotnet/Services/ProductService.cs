using Microsoft.EntityFrameworkCore;
using ShoppingAssistant.Data;
using ShoppingAssistant.DTOs;
using ShoppingAssistant.Models;

namespace ShoppingAssistant.Services;

public class ProductService
{
    private readonly AppDbContext _db;
    public ProductService(AppDbContext db) => _db = db;

    public async Task<PagedResult<ProductDto>> GetAllAsync(int page, int pageSize, string? search, string? category, decimal? maxPrice, string? sortBy)
    {
        var query = _db.Products.Where(p => p.IsActive);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search) || p.Brand.Contains(search) || (p.Tags != null && p.Tags.Contains(search)));

        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category == category);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        query = sortBy switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "rating" => query.OrderByDescending(p => p.Rating),
            "popular" => query.OrderByDescending(p => p.ReviewCount),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<ProductDto>(items.Select(MapToDto).ToList(), total, page, pageSize,
            (int)Math.Ceiling((double)total / pageSize));
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var p = await _db.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        return p == null ? null : MapToDto(p);
    }

    public async Task<List<string>> GetCategoriesAsync()
        => await _db.Products.Where(p => p.IsActive).Select(p => p.Category).Distinct().ToListAsync();

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name, Description = dto.Description, Category = dto.Category,
            Brand = dto.Brand, Price = dto.Price, OriginalPrice = dto.OriginalPrice,
            Stock = dto.Stock, Rating = dto.Rating, ReviewCount = dto.ReviewCount,
            ImageUrl = dto.ImageUrl, Tags = dto.Tags
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return MapToDto(product);
    }

    private static ProductDto MapToDto(Product p) => new(
        p.Id, p.Name, p.Description, p.Category, p.Brand,
        p.Price, p.OriginalPrice, p.Stock, p.Rating, p.ReviewCount,
        p.ImageUrl, p.Tags, p.IsActive);
}
