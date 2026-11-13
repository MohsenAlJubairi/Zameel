using Zameel.Application.Common;
using Zameel.Application.DTOs;

namespace Zameel.Application.Interfaces;

public interface IChatService
{
    Task<ServiceResult<List<ConversationSummaryDto>>> GetConversationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ServiceResult<ChatThreadViewModel>> GetChatThreadAsync(Guid currentUserId, Guid otherUserId, Guid? listingId, CancellationToken cancellationToken = default);
    Task<ServiceResult<ChatMessageDto>> SendMessageAsync(Guid senderId, SendMessageDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> MarkConversationAsReadAsync(Guid currentUserId, Guid otherUserId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadMessagesCountAsync(Guid userId, CancellationToken cancellationToken = default);
}
