namespace Zameel.Application.Common;

public static class AppMessages
{
    // General
    public const string OperationSuccess = "تمت العملية بنجاح.";
    public const string UnexpectedError = "حدث خطأ غير متوقع، يرجى المحاولة لاحقاً.";
    public const string ItemNotFound = "العنصر المطلوب غير موجود أو تم حذفه.";
    public const string UnauthorizedAccess = "ليس لديك الصلاحية لتنفيذ هذا الإجراء.";

    // Quotas & Limits
    public const string QuotaExceededUnverified = "لقد وصلت للحد الأقصى للإعلانات والطلبات النشطة (3 إعلانات). وثّق حسابك ببطاقتك الجامعية لزيادة الحد إلى 5 إعلانات أو قم بتسوية إعلاناتك السابقة كمباعة.";
    public const string QuotaExceededVerified = "لقد وصلت للحد الأقصى للإعلانات والطلبات النشطة (5 إعلانات). يرجى تحديث حالة إعلاناتك السابقة إلى 'مباع' لتتمكن من إضافة إعلانات جديدة.";
    public const string ImageCountExceeded = "الحد الأقصى لعدد الصور لكل إعلان هو 5 صور.";
    public const string ImageSizeExceeded = "حجم الصورة يتجاوز الحد المسموح به (5 ميجابايت).";
    public const string InvalidImageFormat = "صيغة الصورة غير مدعومة. الصيغ المسموحة: JPG, PNG, WEBP.";

    // Listings
    public const string ListingCreatedSuccess = "تم نشر الإعلان بنجاح، نتمنى لك بيعاً موفقاً!";
    public const string ListingPendingStoreApproval = "تم إرسال إعلانك بنجاح! السلعة بحالة 'معلق' وبانتظار تأكيد المكتبة الشريكة بعد إيداعك لها لتصبح 'متاحة' للطلاب في السوق.";
    public const string StoreListingApproved = "تم تأكيد وجود السلعة بنجاح وتحويل الإعلان إلى 'متاح' في السوق الجامعي.";
    public const string StoreListingRejected = "تم رفض طلب الإيداع وإلغاء الإعلان بنجاح.";
    public const string StoreRequiredForListing = "يرجى اختيار إحدى المكتبات الجامعية المعتمدة كنقطة إيداع واستلام للسلعة.";
    public const string ListingUpdatedSuccess = "تم تحديث بيانات الإعلان بنجاح.";
    public const string ListingMarkedAsSold = "تم تمييز المستلزم كمباع وتفريغ مساحة في رصيد إعلاناتك، شكراً لتعاونك!";
    public const string ListingRenewedSuccess = "تم تجديد صلاحية الإعلان بنجاح لمدة 30 يوماً إضافية.";
    public const string ListingReportedSoldSuccess = "شكراً لمساعدتك في إبقاء السوق الجامعي دقيقاً، تم تسجيل إبلاغك.";

    // Auth & Accounts
    public const string OtpSentSuccess = "تم إرسال رمز التحقق إلى بريدك الإلكتروني بنجاح.";
    public const string InvalidOtp = "رمز التحقق غير صحيح أو انتهت صلاحيته.";
    public const string VerificationSubmitted = "تم إرسال طلب التوثيق الأكاديمي بنجاح، وستتم مراجعته وتفعيل شارة التوثيق وتوسيع الكوتا خلال 24 ساعة.";
    public const string AccountVerifiedSuccess = "تهانينا! تم توثيق حسابك الجامعي بنجاح وحصلت على شارة التوثيق وتوسيع كوتا النشر إلى 5 إعلانات.";
    public const string DuplicateStudentRegNo = "رقم القيد الجامعي هذا مسجل وموثق مسبقاً في هذه الجامعة.";

    // Comments & Inquiries
    public const string CommentAddedSuccess = "تمت إضافة تعليقك بنجاح.";
    public const string CommentDeletedSuccess = "تم حذف التعليق بنجاح.";
    public const string CommentContentRequired = "يرجى كتابة نص التعليق أو الاستفسار أولاً.";

    // Chat & Messaging
    public const string MessageSentSuccess = "تم إرسال الرسالة بنجاح.";
    public const string MessageContentRequired = "يرجى كتابة نص الرسالة أولاً.";
    public const string CannotMessageSelf = "لا يمكنك إرسال رسالة إلى نفسك.";
    public const string ReceiverNotFound = "المستخدم المستلم غير موجود أو حسابه غير مفعل.";

    // Deals & Reviews
    public const string DealConfirmedSuccess = "تم تأكيد إتمام المعاملة بنجاح، يمكنك الآن تقييم زميلك.";
    public const string ReviewSubmittedSuccess = "شكراً لتقييمك! تقييماتك تعزز ثقة وأمان مجتمع زميل الجامعي.";

    // Academic Catalog & Universities Management
    public const string UniversityCreatedSuccess = "تمت إضافة الجامعة بنجاح إلى دليل الجامعات.";
    public const string UniversityUpdatedSuccess = "تم تحديث بيانات الجامعة بنجاح.";
    public const string UniversityStatusToggled = "تم تغيير حالة تفعيل الجامعة بنجاح.";
    public const string FacultyCreatedSuccess = "تمت إضافة الكلية بنجاح إلى الدليل الأكاديمي.";
    public const string FacultyUpdatedSuccess = "تم تحديث بيانات الكلية بنجاح.";
    public const string FacultyStatusToggled = "تم تغيير حالة تفعيل الكلية بنجاح.";
    public const string MajorCreatedSuccess = "تمت إضافة التخصص الأكاديمي بنجاح.";
    public const string FacultyLinkedSuccess = "تم ربط الكلية بالجامعة بنجاح.";
    public const string FacultyUnlinkedSuccess = "تم فك ارتباط الكلية بالجامعة بنجاح.";
    public const string MajorLinkedSuccess = "تم ربط التخصص الأكاديمي بالكلية بنجاح.";
    public const string MajorUnlinkedSuccess = "تم فك ارتباط التخصص بالكلية بنجاح.";
    public const string DuplicateAcademicEntry = "هذا السجل الأكاديمي مرتبط أو مسجل مسبقاً.";
}
