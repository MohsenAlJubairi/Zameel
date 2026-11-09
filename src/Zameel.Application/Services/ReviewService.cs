using Microsoft.EntityFrameworkCore;
using Zameel.Application.Common;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;
using Zameel.Domain.Entities;

namespace Zameel.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IApplicationDbContext _context;

    public ReviewService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<bool>> SubmitReviewAsync(SubmitReviewDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.reviewerId == Guid.Empty || dto.revieweeId == Guid.Empty || dto.listingId == Guid.Empty)
        {
            return ServiceResult<bool>.Fail(AppMessages.UnauthorizedAccess);
        }

        if (dto.reviewerId == dto.revieweeId)
        {
            return ServiceResult<bool>.Fail("لا يمكنك تقييم نفسك.");
        }

        if (dto.rating < 1 || dto.rating > 5)
        {
            return ServiceResult<bool>.Fail("يجب أن يكون التقييم بين 1 و 5 نجوم.");
        }

        // Verify that listing belongs to reviewee and reviewer is not reviewing themselves
        var listing = await _context.listings
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.id == dto.listingId && l.userId == dto.revieweeId, cancellationToken);

        if (listing == null)
        {
            return ServiceResult<bool>.Fail("المستلزم المطلوب تقييمه غير موجود أو لا ينتمي لهذا الزميل.");
        }

        if (dto.reviewerId == dto.revieweeId)
        {
            return ServiceResult<bool>.Fail("لا يمكنك تقييم نفسك.");
        }

        // Prevent duplicate reviews
        var alreadyReviewed = await _context.reviews
            .AsNoTracking()
            .AnyAsync(r => r.listingId == dto.listingId && r.reviewerId == dto.reviewerId, cancellationToken);

        if (alreadyReviewed)
        {
            return ServiceResult<bool>.Fail("لقد قمت بتقييم هذا التعامل مسبقاً.");
        }

        var review = new Review
        {
            listingId = dto.listingId,
            reviewerId = dto.reviewerId,
            revieweeId = dto.revieweeId,
            rating = dto.rating,
            comment = dto.comment?.Trim()
        };

        await _context.reviews.AddAsync(review, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true, AppMessages.ReviewSubmittedSuccess);
    }

    public async Task<ServiceResult<List<ReviewItemDto>>> GetUserReviewsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            return ServiceResult<List<ReviewItemDto>>.Fail(AppMessages.UnauthorizedAccess);
        }

        var reviews = await _context.reviews
            .AsNoTracking()
            .Where(r => r.revieweeId == userId)
            .OrderByDescending(r => r.createdAt)
            .Select(r => new ReviewItemDto(
                r.id,
                r.reviewer.fullName,
                r.reviewer.avatarUrl,
                r.rating,
                r.comment,
                r.createdAt
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<ReviewItemDto>>.Success(reviews);
    }
}
