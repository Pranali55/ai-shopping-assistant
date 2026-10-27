using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ShoppingAssistant.Data;
using ShoppingAssistant.DTOs;
using ShoppingAssistant.Models;

namespace ShoppingAssistant.Services;

public class ChatService
{
    private readonly AppDbContext _db;
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public ChatService(AppDbContext db, HttpClient http, IConfiguration config)
    {
        _db = db;
        _http = http;
        _config = config;
    }

    public async Task<ChatSessionDto> StartSessionAsync(int userId, StartChatDto dto)
    {
        var session = new ChatSession
        {
            UserId = userId,
            Title = "Shopping Chat",
        };
        _db.ChatSessions.Add(session);
        await _db.SaveChangesAsync();

        if (!string.IsNullOrEmpty(dto.InitialMessage))
            await SendMessageAsync(userId, session.Id, new SendMessageDto(dto.InitialMessage));

        return await GetSessionAsync(userId, session.Id) ?? throw new Exception("Session not found");
    }

    public async Task<ChatMessageDto> SendMessageAsync(int userId, int sessionId, SendMessageDto dto)
    {
        var session = await _db.ChatSessions
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId)
            ?? throw new Exception("Session not found");

        // Save user message
        var userMessage = new ChatMessage { SessionId = sessionId, Role = "user", Content = dto.Message };
        _db.ChatMessages.Add(userMessage);

        // Update session title from first message
        if (session.Messages.Count == 0)
            session.Title = dto.Message.Length > 40 ? dto.Message[..40] + "..." : dto.Message;

        session.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        // Get all products for AI context
        var products = await _db.Products.Where(p => p.IsActive).ToListAsync();
        var productDtos = products.Select(MapProduct).ToList();

        // Build chat history
        var history = session.Messages
            .OrderBy(m => m.CreatedAt)
            .Select(m => new AiMessage(m.Role, m.Content))
            .ToList();

        // Get user preferences
        var user = await _db.Users.FindAsync(userId);
        UserPreferencesDto? prefs = null;
        if (user != null)
        {
            var cats = user.PreferredCategories != null
                ? JsonSerializer.Deserialize<List<string>>(user.PreferredCategories)
                : null;
            prefs = new UserPreferencesDto(user.BudgetRange, cats);
        }

        // Call AI service
        var aiRequest = new AiChatRequest(dto.Message, history, prefs, productDtos);
        AiChatResponse aiResponse;

        try
        {
            var baseUrl = _config["AiService__BaseUrl"];
            var content = new StringContent(JsonSerializer.Serialize(aiRequest), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync($"{baseUrl}/chat", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            aiResponse = JsonSerializer.Deserialize<AiChatResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new Exception("Invalid AI response");
        }
        catch
        {
            // Fallback response if AI service is down
            aiResponse = new AiChatResponse(
                "I'm here to help you find the perfect product! Could you tell me more about what you're looking for?",
                new List<int>(),
                "unknown",
                new List<string> { "Show me electronics", "Find shoes under $100", "Best rated products" }
            );
        }

        // Save assistant reply
        var recommendedIds = JsonSerializer.Serialize(aiResponse.RecommendedProductIds);
        var assistantMessage = new ChatMessage
        {
            SessionId = sessionId,
            Role = "assistant",
            Content = aiResponse.Reply,
            ProductIds = recommendedIds
        };
        _db.ChatMessages.Add(assistantMessage);
        await _db.SaveChangesAsync();

        // Get recommended products
        List<ProductDto>? recommendedProducts = null;
        if (aiResponse.RecommendedProductIds.Any())
        {
            recommendedProducts = products
                .Where(p => aiResponse.RecommendedProductIds.Contains(p.Id))
                .Select(MapProduct).ToList();
        }

        return new ChatMessageDto(assistantMessage.Id, "assistant", aiResponse.Reply, recommendedProducts, assistantMessage.CreatedAt);
    }

    public async Task<List<ChatSessionSummaryDto>> GetSessionsAsync(int userId)
    {
        return await _db.ChatSessions
            .Include(s => s.Messages)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.UpdatedAt)
            .Select(s => new ChatSessionSummaryDto(s.Id, s.Title, s.CreatedAt, s.UpdatedAt, s.Messages.Count))
            .ToListAsync();
    }

    public async Task<ChatSessionDto?> GetSessionAsync(int userId, int sessionId)
    {
        var session = await _db.ChatSessions
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);
        if (session == null) return null;

        var products = await _db.Products.Where(p => p.IsActive).ToListAsync();
        var messages = session.Messages.OrderBy(m => m.CreatedAt).Select(m =>
        {
            List<ProductDto>? recs = null;
            if (!string.IsNullOrEmpty(m.ProductIds))
            {
                var ids = JsonSerializer.Deserialize<List<int>>(m.ProductIds) ?? new();
                recs = products.Where(p => ids.Contains(p.Id)).Select(MapProduct).ToList();
            }
            return new ChatMessageDto(m.Id, m.Role, m.Content, recs, m.CreatedAt);
        }).ToList();

        return new ChatSessionDto(session.Id, session.Title, session.CreatedAt, session.UpdatedAt, messages);
    }

    public async Task<bool> DeleteSessionAsync(int userId, int sessionId)
    {
        var session = await _db.ChatSessions.FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);
        if (session == null) return false;
        _db.ChatSessions.Remove(session);
        await _db.SaveChangesAsync();
        return true;
    }

    private static ProductDto MapProduct(Product p) => new(
        p.Id, p.Name, p.Description, p.Category, p.Brand,
        p.Price, p.OriginalPrice, p.Stock, p.Rating, p.ReviewCount,
        p.ImageUrl, p.Tags, p.IsActive);
}
