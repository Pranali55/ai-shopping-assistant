using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingAssistant.DTOs;
using ShoppingAssistant.Services;

namespace ShoppingAssistant.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly ChatService _chat;
    public ChatController(ChatService chat) => _chat = chat;
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetSessions()
        => Ok(await _chat.GetSessionsAsync(UserId));

    [HttpPost]
    public async Task<IActionResult> StartSession(StartChatDto dto)
        => Ok(await _chat.StartSessionAsync(UserId, dto));

    [HttpGet("{sessionId}")]
    public async Task<IActionResult> GetSession(int sessionId)
    {
        var session = await _chat.GetSessionAsync(UserId, sessionId);
        return session == null ? NotFound() : Ok(session);
    }

    [HttpPost("{sessionId}/messages")]
    public async Task<IActionResult> SendMessage(int sessionId, SendMessageDto dto)
    {
        try { return Ok(await _chat.SendMessageAsync(UserId, sessionId, dto)); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpDelete("{sessionId}")]
    public async Task<IActionResult> DeleteSession(int sessionId)
    {
        var deleted = await _chat.DeleteSessionAsync(UserId, sessionId);
        return deleted ? NoContent() : NotFound();
    }
}
