using Microsoft.Extensions.DependencyInjection;
using Zameel.Application.Interfaces;
using Zameel.Application.Services;

namespace Zameel.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAcademicCatalogService, AcademicCatalogService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IQuotaService, QuotaService>();
        services.AddScoped<IListingService, ListingService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IChatService, ChatService>();

        return services;
    }
}
