namespace Zameel.Domain.Constants;

public static class SystemConstants
{
    // Currency (العملة الرسمية الموحدة)
    public const string Currency = "ريال يمني";
    public const string CurrencySymbol = "ر.ي";

    // Quotas (سقوف النشر المشتركة الافتراضية)
    public const int UnverifiedStudentMaxActiveListings = 3;
    public const int VerifiedStudentMaxActiveListings = 5;

    // Lifecycles (دورة حياة الإعلان)
    public const int AutoExpirationDays = 30;
    public const int SoldReportsThresholdToHide = 2;
    public const int DealConfirmationPromptHours = 48;

    // Pagination (التصفح الافتراضي)
    public const int DefaultPageSize = 12;
    public const int MaxPageSize = 50;

    // Image Limits
    public const int MaxImagesPerListing = 5;
    public const long MaxImageFileSizeInBytes = 5 * 1024 * 1024; // 5MB
}

public static class SystemRoles
{
    public const string Admin = "ADMIN";
    public const string Moderator = "MODERATOR";
    public const string StudentVerified = "STUDENT_VERIFIED";
    public const string StudentUnverified = "STUDENT_UNVERIFIED";
    public const string Store = "STORE";
}

public static class SystemPermissions
{
    // Listings & Moderation
    public const string ListingsApprove = "listings.approve";
    public const string ListingsDelete = "listings.delete";
    public const string ListingsManage = "listings.manage";

    // Users & Verifications
    public const string UsersManage = "users.manage";
    public const string UsersVerify = "users.verify";

    // Stores
    public const string StoresManage = "stores.manage";

    // Roles & Plans Management
    public const string RolesManage = "roles.manage";
    public const string PlansManage = "plans.manage";

    // Academic Catalog
    public const string CatalogManage = "catalog.manage";
}
