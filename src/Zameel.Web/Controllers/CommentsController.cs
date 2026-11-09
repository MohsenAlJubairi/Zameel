using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zameel.Application.Common;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;

namespace Zameel.Web.Controllers;

[Route("comments")]
[Authorize]
public class CommentsController : Controller
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : null;
    }

    [HttpPost("add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(
        [FromForm] Guid listingId,
        [FromForm] string content,
        [FromForm] Guid? parentCommentId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Challenge();
        }

        var dto = new CreateCommentDto(listingId, content, parentCommentId);
        var result = await _commentService.AddCommentAsync(dto, userId.Value, cancellationToken);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(result);
        }

        if (result.isSuccess)
        {
            TempData["SuccessMessage"] = result.message;
        }
        else
        {
            TempData["ErrorMessage"] = result.message;
        }

        return Redirect($"/listings/{listingId}#comments");
    }

    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, [FromForm] Guid listingId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Challenge();
        }

        var result = await _commentService.DeleteCommentAsync(id, userId.Value, cancellationToken);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(result);
        }

        if (result.isSuccess)
        {
            TempData["SuccessMessage"] = result.message;
        }
        else
        {
            TempData["ErrorMessage"] = result.message;
        }

        return Redirect($"/listings/{listingId}#comments");
    }
}
