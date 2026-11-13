namespace Zameel.Application.DTOs;

public record SendMessageDto(
    Guid receiverId,
    Guid? listingId,
    string content
);

public record ChatMessageDto(
    Guid id,
    Guid senderId,
    string senderName,
    string? senderAvatarUrl,
    Guid receiverId,
    string receiverName,
    Guid? listingId,
    string? listingTitle,
    decimal? listingPrice,
    string? listingImageUrl,
    string content,
    bool isRead,
    DateTime createdAt,
    bool isMine
);

public record ConversationSummaryDto(
    Guid otherUserId,
    string otherUserName,
    string? otherUserAvatarUrl,
    bool otherUserIsVerified,
    string lastMessageContent,
    DateTime lastMessageTime,
    int unreadCount,
    Guid? lastListingId,
    string? lastListingTitle
);

public record ChatThreadViewModel(
    Guid currentUserId,
    Guid otherUserId,
    string otherUserName,
    string? otherUserAvatarUrl,
    bool otherUserIsVerified,
    Guid? listingId,
    string? listingTitle,
    decimal? listingPrice,
    string? listingImageUrl,
    List<ChatMessageDto> messages
);
