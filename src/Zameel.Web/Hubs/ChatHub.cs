using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;

namespace Zameel.Web.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task JoinConversation(string otherUserIdStr)
    {
        if (Guid.TryParse(otherUserIdStr, out var otherUserId) && TryGetCurrentUserId(out var currentUserId))
        {
            var groupName = GetConversationGroupName(currentUserId, otherUserId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }

    public async Task LeaveConversation(string otherUserIdStr)
    {
        if (Guid.TryParse(otherUserIdStr, out var otherUserId) && TryGetCurrentUserId(out var currentUserId))
        {
            var groupName = GetConversationGroupName(currentUserId, otherUserId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }

    public async Task SendMessage(string receiverIdStr, string content, string? listingIdStr)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return;
        }

        if (!Guid.TryParse(receiverIdStr, out var receiverId))
        {
            return;
        }

        Guid? listingId = null;
        if (!string.IsNullOrEmpty(listingIdStr) && Guid.TryParse(listingIdStr, out var parsedListingId))
        {
            listingId = parsedListingId;
        }

        var dto = new SendMessageDto(receiverId, listingId, content);
        var result = await _chatService.SendMessageAsync(currentUserId, dto);

        if (result.isSuccess && result.data != null)
        {
            var groupName = GetConversationGroupName(currentUserId, receiverId);
            
            // Broadcast to conversation participants in the active room
            await Clients.Group(groupName).SendAsync("ReceiveMessage", result.data);

            // Also broadcast global unread ping to receiver's personal user stream
            await Clients.User(receiverId.ToString()).SendAsync("NewMessagePing", new
            {
                senderId = currentUserId,
                senderName = result.data.senderName,
                content = result.data.content,
                createdAt = result.data.createdAt
            });
        }
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        userId = Guid.Empty;
        var claim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out userId);
    }

    private static string GetConversationGroupName(Guid u1, Guid u2)
    {
        return u1.CompareTo(u2) < 0 
            ? $"chat_{u1}_{u2}" 
            : $"chat_{u2}_{u1}";
    }
}
