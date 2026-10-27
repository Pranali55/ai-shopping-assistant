using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingAssistant.DTOs;
using ShoppingAssistant.Services;

namespace ShoppingAssistant.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _products;
    public ProductsController(ProductService products) => _products = products;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 12,
        [FromQuery] string? search = null, [FromQuery] string? category = null,
        [FromQuery] decimal? maxPrice = null, [FromQuery] string? sortBy = null)
        => Ok(await _products.GetAllAsync(page, pageSize, search, category, maxPrice, sortBy));

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
        => Ok(await _products.GetCategoriesAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _products.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost, Authorize]
    public async Task<IActionResult> Create(CreateProductDto dto)
        => Ok(await _products.CreateAsync(dto));
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly CartWishlistService _service;
    public CartController(CartWishlistService service) => _service = service;
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet] public async Task<IActionResult> Get() => Ok(await _service.GetCartAsync(UserId));
    [HttpPost] public async Task<IActionResult> Add(AddToCartDto dto) => Ok(await _service.AddToCartAsync(UserId, dto));
    [HttpDelete("{id}")] public async Task<IActionResult> Remove(int id) => await _service.RemoveFromCartAsync(UserId, id) ? NoContent() : NotFound();
    [HttpDelete] public async Task<IActionResult> Clear() { await _service.ClearCartAsync(UserId); return NoContent(); }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly CartWishlistService _service;
    public WishlistController(CartWishlistService service) => _service = service;
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet] public async Task<IActionResult> Get() => Ok(await _service.GetWishlistAsync(UserId));
    [HttpPost] public async Task<IActionResult> Add(AddToWishlistDto dto) => await _service.AddToWishlistAsync(UserId, dto) ? Ok() : BadRequest("Already in wishlist.");
    [HttpDelete("{id}")] public async Task<IActionResult> Remove(int id) => await _service.RemoveFromWishlistAsync(UserId, id) ? NoContent() : NotFound();
}
