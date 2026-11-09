using Microsoft.EntityFrameworkCore;
using Zameel.Application.Common;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;
using Zameel.Domain.Constants;
using Zameel.Domain.Entities;
using Zameel.Domain.Enums;

namespace Zameel.Application.Services;

public class CommentService : ICommentService
{
    private readonly IApplicationDbContext _context;

    public CommentService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<List<CommentDto>>> GetListingCommentsAsync(Guid listingId, Guid? currentUserId, CancellationToken cancellationToken = default)
    {
        var listing = await _context.listings
            .AsNoTracking()
            .Select(l => new { l.id, l.userId })
            .FirstOrDefaultAsync(l => l.id == listingId, cancellationToken);

        if (listing == null)
        {
            return ServiceResult<List<CommentDto>>.Fail(AppMessages.ItemNotFound);
        }

        var currentUser = currentUserId.HasValue
            ? await _context.users.AsNoTracking().Select(u => new { u.id, RoleNormalized = u.role.normalizedName }).FirstOrDefaultAsync(u => u.id == currentUserId.Value, cancellationToken)
            : null;

        bool isAdmin = currentUser?.RoleNormalized == SystemRoles.Admin;
        bool isCurrentSeller = currentUserId.HasValue && listing.userId == currentUserId.Value;

        var allComments = await _context.comments
            .AsNoTracking()
            .Where(c => c.listingId == listingId)
            .OrderBy(c => c.createdAt)
            .Select(c => new
            {
                c.id,
                c.listingId,
                c.userId,
                authorName = c.user.fullName,
                isAuthorVerified = c.user.isVerified,
                authorRole = c.user.role.name,
                c.content,
                c.createdAt,
                c.parentCommentId
            })
            .ToListAsync(cancellationToken);

        var repliesLookup = allComments
            .Where(c => c.parentCommentId != null)
            .ToLookup(c => c.parentCommentId!.Value);

        var rootComments = allComments
            .Where(c => c.parentCommentId == null)
            .Select(c =>
            {
                var replies = repliesLookup[c.id].Select(r => new CommentDto(
                    r.id,
                    r.listingId,
                    r.userId,
                    r.authorName,
                    r.isAuthorVerified,
                    r.authorRole,
                    r.content,
                    r.createdAt,
                    r.parentCommentId,
                    new List<CommentDto>(),
                    r.userId == listing.userId,
                    currentUserId.HasValue && (r.userId == currentUserId.Value || isCurrentSeller || isAdmin)
                )).ToList();

                return new CommentDto(
                    c.id,
                    c.listingId,
                    c.userId,
                    c.authorName,
                    c.isAuthorVerified,
                    c.authorRole,
                    c.content,
                    c.createdAt,
                    c.parentCommentId,
                    replies,
                    c.userId == listing.userId,
                    currentUserId.HasValue && (c.userId == currentUserId.Value || isCurrentSeller || isAdmin)
                );
            })
            .ToList();

        return ServiceResult<List<CommentDto>>.Success(rootComments);
    }

    public async Task<ServiceResult<CommentDto>> AddCommentAsync(CreateCommentDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.content))
        {
            return ServiceResult<CommentDto>.Fail(AppMessages.CommentContentRequired);
        }

        var content = dto.content.Trim();
        if (content.Length < 2)
        {
            return ServiceResult<CommentDto>.Fail("نص التعليق قصير جداً.");
        }

        if (content.Length > 1000)
        {
            return ServiceResult<CommentDto>.Fail("يتجاوز التعليق الحد الأقصى المسموح به (1000 حرف).");
        }

        var listing = await _context.listings
            .AsNoTracking()
            .Select(l => new { l.id, l.userId })
            .FirstOrDefaultAsync(l => l.id == dto.listingId, cancellationToken);

        if (listing == null)
        {
            return ServiceResult<CommentDto>.Fail(AppMessages.ItemNotFound);
        }

        if (dto.parentCommentId.HasValue)
        {
            var parentExists = await _context.comments
                .AnyAsync(c => c.id == dto.parentCommentId.Value && c.listingId == dto.listingId, cancellationToken);

            if (!parentExists)
            {
                return ServiceResult<CommentDto>.Fail("التعليق المراد الرد عليه غير موجود.");
            }
        }

        var user = await _context.users
            .AsNoTracking()
            .Select(u => new { u.id, u.fullName, u.isVerified, RoleName = u.role.name })
            .FirstOrDefaultAsync(u => u.id == userId, cancellationToken);

        if (user == null)
        {
            return ServiceResult<CommentDto>.Fail(AppMessages.UnauthorizedAccess);
        }

        var comment = new Comment
        {
            listingId = dto.listingId,
            userId = userId,
            parentCommentId = dto.parentCommentId,
            content = content,
            createdAt = DateTime.UtcNow
        };

        await _context.comments.AddAsync(comment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var resultDto = new CommentDto(
            comment.id,
            comment.listingId,
            comment.userId,
            user.fullName,
            user.isVerified,
            user.RoleName,
            comment.content,
            comment.createdAt,
            comment.parentCommentId,
            new List<CommentDto>(),
            user.id == listing.userId,
            true
        );

        return ServiceResult<CommentDto>.Success(resultDto, AppMessages.CommentAddedSuccess);
    }

    public async Task<ServiceResult<bool>> DeleteCommentAsync(Guid commentId, Guid userId, CancellationToken cancellationToken = default)
    {
        var comment = await _context.comments
            .FirstOrDefaultAsync(c => c.id == commentId, cancellationToken);

        if (comment == null)
        {
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);
        }

        var listing = await _context.listings
            .AsNoTracking()
            .Select(l => new { l.id, l.userId })
            .FirstOrDefaultAsync(l => l.id == comment.listingId, cancellationToken);

        var user = await _context.users
            .AsNoTracking()
            .Select(u => new { u.id, RoleNormalized = u.role.normalizedName })
            .FirstOrDefaultAsync(u => u.id == userId, cancellationToken);

        bool isAdmin = user?.RoleNormalized == SystemRoles.Admin;
        bool isSeller = listing != null && listing.userId == userId;
        bool isAuthor = comment.userId == userId;

        if (!isAuthor && !isSeller && !isAdmin)
        {
            return ServiceResult<bool>.Fail(AppMessages.UnauthorizedAccess);
        }

        comment.isDeleted = true;
        comment.updatedAt = DateTime.UtcNow;

        // Also soft delete child replies
        var childReplies = await _context.comments
            .Where(c => c.parentCommentId == commentId)
            .ToListAsync(cancellationToken);

        foreach (var reply in childReplies)
        {
            reply.isDeleted = true;
            reply.updatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true, AppMessages.CommentDeletedSuccess);
    }
}
