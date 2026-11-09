using Zameel.Application.Common;
using Zameel.Application.DTOs;

namespace Zameel.Application.Interfaces;

public interface ICommentService
{
    Task<ServiceResult<List<CommentDto>>> GetListingCommentsAsync(Guid listingId, Guid? currentUserId, CancellationToken cancellationToken = default);
    Task<ServiceResult<CommentDto>> AddCommentAsync(CreateCommentDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> DeleteCommentAsync(Guid commentId, Guid userId, CancellationToken cancellationToken = default);
}
