using Microsoft.EntityFrameworkCore;
using Zameel.Application.Common;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;
using Zameel.Domain.Entities;

namespace Zameel.Application.Services;

public class ChatService : IChatService
{
    private readonly IApplicationDbContext _context;

    public ChatService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<List<ConversationSummaryDto>>> GetConversationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            return ServiceResult<List<ConversationSummaryDto>>.Fail(AppMessages.UnauthorizedAccess);
        }

        // Get all messages where user is either sender or receiver
        var messages = await _context.chatMessages
            .AsNoTracking()
            .Include(m => m.sender)
            .Include(m => m.receiver)
            .Include(m => m.listing)
            .Where(m => m.senderId == userId || m.receiverId == userId)
            .OrderByDescending(m => m.createdAt)
            .ToListAsync(cancellationToken);

        // Group by the other user ID
        var conversationGroups = messages
            .GroupBy(m => m.senderId == userId ? m.receiverId : m.senderId);

        var result = new List<ConversationSummaryDto>();

        foreach (var group in conversationGroups)
        {
            var otherUserId = group.Key;
            var latestMessage = group.First();

            var otherUser = latestMessage.senderId == otherUserId 
                ? latestMessage.sender 
                : latestMessage.receiver;

            if (otherUser == null) continue;

            var unreadCount = group.Count(m => m.receiverId == userId && !m.isRead);

            // Latest contextual listing in the conversation
            var latestListing = group.FirstOrDefault(m => m.listing != null)?.listing;

            result.Add(new ConversationSummaryDto(
                otherUserId: otherUserId,
                otherUserName: otherUser.fullName,
                otherUserAvatarUrl: otherUser.avatarUrl,
                otherUserIsVerified: otherUser.isVerified,
                lastMessageContent: latestMessage.content,
                lastMessageTime: latestMessage.createdAt,
                unreadCount: unreadCount,
                lastListingId: latestListing?.id,
                lastListingTitle: latestListing?.title
            ));
        }

        result = result.OrderByDescending(c => c.lastMessageTime).ToList();

        return ServiceResult<List<ConversationSummaryDto>>.Success(result);
    }

    public async Task<ServiceResult<ChatThreadViewModel>> GetChatThreadAsync(
        Guid currentUserId, 
        Guid otherUserId, 
        Guid? listingId, 
        CancellationToken cancellationToken = default)
    {
        if (currentUserId == Guid.Empty)
        {
            return ServiceResult<ChatThreadViewModel>.Fail(AppMessages.UnauthorizedAccess);
        }

        if (otherUserId == Guid.Empty || otherUserId == currentUserId)
        {
            return ServiceResult<ChatThreadViewModel>.Fail(AppMessages.CannotMessageSelf);
        }

        var otherUser = await _context.users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.id == otherUserId, cancellationToken);

        if (otherUser == null)
        {
            return ServiceResult<ChatThreadViewModel>.Fail(AppMessages.ReceiverNotFound);
        }

        // Get Listing info if specified
        Listing? targetListing = null;
        if (listingId.HasValue && listingId.Value != Guid.Empty)
        {
            targetListing = await _context.listings
                .AsNoTracking()
                .Include(l => l.images)
                .FirstOrDefaultAsync(l => l.id == listingId.Value, cancellationToken);
        }

        // Retrieve messages between these two users
        var messages = await _context.chatMessages
            .AsNoTracking()
            .Include(m => m.sender)
            .Include(m => m.receiver)
            .Include(m => m.listing).ThenInclude(l => l!.images)
            .Where(m => (m.senderId == currentUserId && m.receiverId == otherUserId) ||
                        (m.senderId == otherUserId && m.receiverId == currentUserId))
            .OrderBy(m => m.createdAt)
            .Take(100)
            .ToListAsync(cancellationToken);

        // Map to ChatMessageDto
        var mappedMessages = messages.Select(m => new ChatMessageDto(
            id: m.id,
            senderId: m.senderId,
            senderName: m.sender.fullName,
            senderAvatarUrl: m.sender.avatarUrl,
            receiverId: m.receiverId,
            receiverName: m.receiver.fullName,
            listingId: m.listingId,
            listingTitle: m.listing?.title,
            listingPrice: m.listing?.price,
            listingImageUrl: m.listing?.images.FirstOrDefault(i => i.isPrimary)?.imageUrl ?? m.listing?.images.FirstOrDefault()?.imageUrl,
            content: m.content,
            isRead: m.isRead,
            createdAt: m.createdAt,
            isMine: m.senderId == currentUserId
        )).ToList();

        // If target listing wasn't in query params, see if the conversation had an associated listing
        if (targetListing == null)
        {
            var msgWithListing = messages.LastOrDefault(m => m.listing != null);
            if (msgWithListing?.listing != null)
            {
                targetListing = msgWithListing.listing;
            }
        }

        string? primaryImgUrl = targetListing?.images.FirstOrDefault(i => i.isPrimary)?.imageUrl 
            ?? targetListing?.images.FirstOrDefault()?.imageUrl;

        var viewModel = new ChatThreadViewModel(
            currentUserId: currentUserId,
            otherUserId: otherUserId,
            otherUserName: otherUser.fullName,
            otherUserAvatarUrl: otherUser.avatarUrl,
            otherUserIsVerified: otherUser.isVerified,
            listingId: targetListing?.id,
            listingTitle: targetListing?.title,
            listingPrice: targetListing?.price,
            listingImageUrl: primaryImgUrl,
            messages: mappedMessages
        );

        // Mark incoming messages as read in the background
        await MarkConversationAsReadAsync(currentUserId, otherUserId, cancellationToken);

        return ServiceResult<ChatThreadViewModel>.Success(viewModel);
    }

    public async Task<ServiceResult<ChatMessageDto>> SendMessageAsync(
        Guid senderId, 
        SendMessageDto dto, 
        CancellationToken cancellationToken = default)
    {
        if (senderId == Guid.Empty)
        {
            return ServiceResult<ChatMessageDto>.Fail(AppMessages.UnauthorizedAccess);
        }

        if (dto.receiverId == Guid.Empty || dto.receiverId == senderId)
        {
            return ServiceResult<ChatMessageDto>.Fail(AppMessages.CannotMessageSelf);
        }

        var content = dto.content?.Trim();
        if (string.IsNullOrWhiteSpace(content))
        {
            return ServiceResult<ChatMessageDto>.Fail(AppMessages.MessageContentRequired);
        }

        if (content.Length > 1500)
        {
            content = content.Substring(0, 1500);
        }

        var sender = await _context.users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.id == senderId, cancellationToken);

        var receiver = await _context.users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.id == dto.receiverId, cancellationToken);

        if (sender == null || receiver == null)
        {
            return ServiceResult<ChatMessageDto>.Fail(AppMessages.ReceiverNotFound);
        }

        // Validate listing if supplied
        Listing? listing = null;
        if (dto.listingId.HasValue && dto.listingId.Value != Guid.Empty)
        {
            listing = await _context.listings
                .AsNoTracking()
                .Include(l => l.images)
                .FirstOrDefaultAsync(l => l.id == dto.listingId.Value, cancellationToken);
        }

        var chatMessage = new ChatMessage
        {
            senderId = senderId,
            receiverId = dto.receiverId,
            listingId = listing?.id,
            content = content,
            isRead = false,
            createdAt = DateTime.UtcNow
        };

        _context.chatMessages.Add(chatMessage);
        await _context.SaveChangesAsync(cancellationToken);

        string? primaryImgUrl = listing?.images.FirstOrDefault(i => i.isPrimary)?.imageUrl 
            ?? listing?.images.FirstOrDefault()?.imageUrl;

        var messageDto = new ChatMessageDto(
            id: chatMessage.id,
            senderId: senderId,
            senderName: sender.fullName,
            senderAvatarUrl: sender.avatarUrl,
            receiverId: dto.receiverId,
            receiverName: receiver.fullName,
            listingId: listing?.id,
            listingTitle: listing?.title,
            listingPrice: listing?.price,
            listingImageUrl: primaryImgUrl,
            content: chatMessage.content,
            isRead: false,
            createdAt: chatMessage.createdAt,
            isMine: true
        );

        return ServiceResult<ChatMessageDto>.Success(messageDto);
    }

    public async Task<ServiceResult<bool>> MarkConversationAsReadAsync(
        Guid currentUserId, 
        Guid otherUserId, 
        CancellationToken cancellationToken = default)
    {
        if (currentUserId == Guid.Empty || otherUserId == Guid.Empty)
        {
            return ServiceResult<bool>.Fail(AppMessages.UnauthorizedAccess);
        }

        var unreadMessages = await _context.chatMessages
            .Where(m => m.receiverId == currentUserId && m.senderId == otherUserId && !m.isRead)
            .ToListAsync(cancellationToken);

        if (unreadMessages.Count > 0)
        {
            var now = DateTime.UtcNow;
            foreach (var msg in unreadMessages)
            {
                msg.isRead = true;
                msg.readAt = now;
                msg.updatedAt = now;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        return ServiceResult<bool>.Success(true);
    }

    public async Task<int> GetUnreadMessagesCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty) return 0;

        return await _context.chatMessages
            .AsNoTracking()
            .CountAsync(m => m.receiverId == userId && !m.isRead, cancellationToken);
    }
}
