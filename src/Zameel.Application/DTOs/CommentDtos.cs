using Zameel.Domain.Enums;

namespace Zameel.Application.DTOs;

public record CommentDto(
    Guid id,
    Guid listingId,
    Guid userId,
    string authorName,
    bool isAuthorVerified,
    string authorRole,
    string content,
    DateTime createdAt,
    Guid? parentCommentId,
    List<CommentDto> replies,
    bool isSeller,
    bool canDelete
);

public record CreateCommentDto(
    Guid listingId,
    string content,
    Guid? parentCommentId = null
);
