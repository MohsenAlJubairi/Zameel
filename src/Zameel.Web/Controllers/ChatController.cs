using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;

namespace Zameel.Web.Controllers;

[Authorize]
[Route("[controller]")]
public class ChatController : Controller
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = "/chat" });
        }

        var result = await _chatService.GetConversationsAsync(currentUserId, cancellationToken);
        if (!result.isSuccess)
        {
            TempData["ErrorMessage"] = result.message;
            return View(new List<ConversationSummaryDto>());
        }

        return View(result.data);
    }

    [HttpGet("{otherUserId:guid}")]
    public async Task<IActionResult> Thread(Guid otherUserId, [FromQuery] Guid? listingId, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = $"/chat/{otherUserId}" });
        }

        if (currentUserId == otherUserId)
        {
            TempData["ErrorMessage"] = "لا يمكنك فتح محادثة مع نفسك.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _chatService.GetChatThreadAsync(currentUserId, otherUserId, listingId, cancellationToken);
        if (!result.isSuccess || result.data == null)
        {
            TempData["ErrorMessage"] = result.message;
            return RedirectToAction(nameof(Index));
        }

        return View(result.data);
    }

    [HttpPost("send")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send([FromBody] SendMessageDto dto, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await _chatService.SendMessageAsync(currentUserId, dto, cancellationToken);
        return Json(result);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount(CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            return Json(new { count = 0 });
        }

        var count = await _chatService.GetUnreadMessagesCountAsync(currentUserId, cancellationToken);
        return Json(new { count });
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }
}
