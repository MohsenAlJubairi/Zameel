# مشروع "زميل" (Zameel) - الدستور البرمجي ومعايير التطوير (Engineering Standards)

## المبادئ الأساسية الحاكمة
1. **KISS (Keep It Simple, Stupid):** أبسط حل مباشر هو الأفضل دائماً. يمنع التعقيد المسبق (No Premature Abstraction).
2. **الاستئذان الإلزامي:** يُمنع تثبيت أي حزم خارجية أو إضافة مكتبات دون استئذان وموافقة مسبقة.

---

## 1. Skill 01: Pragmatic Clean Architecture & KISS
- **تقسيم الطبقات الأربع ببساطة (.NET 10 Solution):**
  1. `Zameel.Domain`: الكيانات (Entities)، الـ Enums، والثوابت. خالٍ تماماً من المكتبات الخارجية.
  2. `Zameel.Application`: الخدمات المباشرة (`Services`)، الـ `DTOs`، والتحويل اليدوي الصريح (Manual Mapping). يعتمد على `IApplicationDbContext`. يُمنع استخدام MediatR أو CQRS أو AutoMapper أو Generic Repository.
  3. `Zameel.Infrastructure`: `ApplicationDbContext`، إعدادات الـ Fluent API، خدمات الملفات (WebP)، والمهام المجدولة الخفيفة (`BackgroundService`).
  4. `Zameel.Web`: وحدات التحكم (Controllers)، صفحات Razor Views، و SignalR Hubs المخصصة فقط. خالية تماماً من منطق الأعمال.

---

## 2. Skill 02: EF Core & SQL Server Standards
- **Clean Entities & Fluent API Only:** كلاسات الـ Entity في `Domain` نظيفة وخالية من Data Annotations؛ كل الإعدادات والعلاقات تُكتب في `Infrastructure` عبر `IEntityTypeConfiguration<T>`.
- **DeleteBehavior.Restrict:** يُمنع الحذف المتسلسل (`Cascade Delete`)؛ استخدم `DeleteBehavior.Restrict` دائماً لحماية البيانات وتجنب أخطاء الدورات في SQL Server.
- **Global Query Filter للـ Soft Delete:** تطبيق تصفية تلقائية `e => !e.isDeleted` على جميع الجداول التي ترث من `BaseEntity`.
- **أداء الاستعلامات:**
  - استخدام `.AsNoTracking()` لكل استعلامات القراءة والعرض.
  - استخدام `.Select()` للإسقاط المباشر على DTOs لمنع جلب حقول غير مطلوبة.
  - منع الاستعلامات غير المحدودة؛ استخدام `.Skip().Take()` دائماً.
  - منع مشكلة N+1 بعدم وضع استعلامات داخل حلقات تكرار.
- **الفهارس المركبة (Composite Indexes):** إنشاء فهارس في Fluent API لأعمدة البحث المتكررة (مثل: `universityId`, `facultyId`, `status`, `createdAt`).

---

## 3. Skill 03: Code Quality, Dynamic Security & ServiceResult
- **الخروج المبكر (Guard Clauses):** التحقق من الشروط والأخطاء أولاً والخروج المبكر لمنع تداخل كتل `if`.
- **نمط النتيجة الموحد (`ServiceResult<T>`):** يُمنع استخدام `throw Exception` لأخطاء الأعمال؛ يتم إرجاع `ServiceResult<T>` بنجاح/فشل ورسالة واضحة.
- **فصل الصلاحيات (RBAC) عن الكوتا (Quotas):**
  - الصلاحيات عبر `[HasPermission]`.
  - الكوتا عبر `QuotaService` موحد يتحقق من السقف المشترك للإعلانات النشطة وطلبات "مطلوب" (3 لغير الموثق / 5 للموثق).
- **أمان رفع الملفات وضغط الصور:**
  - التحقق من الامتداد وحجم الملف وتسميته بـ `Guid.NewGuid()`.
  - ضغط الصور وتحويلها تلقائياً إلى صيغة `WebP` خفيفة لتسريع التصفح لطلاب الجامعات.
- **حماية المعدل (Rate Limiting):** تفعيل محدد معدل خفيف مدمج في .NET لمنع إغراق طلبات OTP والرسائل.

---

## 4. Skill 04: Mobile-First MVC UI, PWA & Frontend KISS
- **الخادم أولاً (SSR):** Razor Views + Partial Views + ViewComponents مع Vanilla JavaScript و `Fetch API` البسيط. يُمنع إدخال أطر SPA ثقيلة كـ React أو Vue.
- **Mobile-First & RTL:** تصميم الهاتف أولاً بدعم كامل للعربية (`dir="rtl"`). الأزرار سهلة اللمس بالإبهام (Thumb-Friendly بحد أدنى 44px). شريط تنقل سفلي (Bottom Navigation Bar).
- **مكونات قابلة لإعادة الاستخدام:** كود بطاقة الإعلان موحد في `_ListingCardPartial.cshtml`.
- **منع الإرسال المكرر:** تعطيل الزر فور الضغط وتغيير نصه إلى "جاري الحفظ...".
- **مشاركة واتساب (OpenGraph Tags):** توليد وسوم `og:title`, `og:description`, `og:image` ديناميكياً لتظهر السلعة بصورتها عند لصق الرابط في واتساب.
- **تخصيص SignalR:** فتح اتصال SignalR فقط في شاشات المحادثة وشريط التنبيهات.
- **PWA Minimal:** ملف `manifest.json` لتثبيت التطبيق على الجوال، و Service Worker خفيف لكاش الأصول وصفحة عدم الاتصال.

---

## 5. قاعدة المركزية الشاملة (Strict Centralization Principle)
- **الألوان والتنسيقات:** تُعرف لوحة الألوان وتدرجاتها وخطوط النظام مركزياً في متغيرات CSS الجذرية (`:root { --primary: ... }`) داخل ملف CSS رئيسي واحد، ويتم توريثها واستدعاؤها في كل المكونات.
- **الرسائل والتنبيهات الموحدة:** جميع رسائل النظام، نصوص الخطأ، إشعارات النجاح، ونصوص التنبيهات تُعرّف في كلاس مركزي واحد (`AppMessages` أو `SystemAlerts`) في طبقة `Application`، ويُمنع كتابة نصوص ثابتة (Hardcoded Strings) مكررة داخل الـ Controllers أو الخدمات.
- **الثوابت والحالات:** جميع حدود الكوتا، مسميات الأدوار، وحالات الإعلانات تُعرف في Enums وثوابت مركزية في `Domain`.

---

## 6. قاعدة تسمية الكمل (camelCase Naming Convention)
- **قاعدة البيانات:** تسمية جميع الجداول والأعمدة في SQL Server تعتمد صيغة **`camelCase`** (مثل: `listingId`, `userId`, `universityId`, `facultyName`, `createdAt`, `isActive`).
- **تكوين الـ EF Core:** يتم ضبط Fluent API أو ModelBuilder Conventions لإنتاج أسماء الأعمدة والجداول بصيغة `camelCase` تلقائياً ومطابقتها.
- **واجهات الـ JSON والـ APIs:** إرجاع حقول الـ JSON بصيغة `camelCase` الصريحة دائماً.
