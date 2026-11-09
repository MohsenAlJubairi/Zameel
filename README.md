# منصة "زميل" (Zameel) - السوق الجامعي الذكي
> **Zameel: Smart Peer-to-Peer Campus Marketplace for University Students**

---

## 📖 نبذة عن المنصة
**زميل** هي منصة ويب تقدم نموذج السوق الجامعي الذكي لتبادل المستلزمات والمقررات والمعدات الدراسية يداً بيد بين الطلاب داخل الحرم الجامعي، مدعومة بنقاط استلام آمنة لدى المكتبات الجامعية المعتمدة (Campus Bookstore PUDO Hub).

---

## 🌟 أبرز المميزات (Key Features)

1. **نموذج الإيداع والاستلام بالمكتبات الجامعية (Campus Bookstore PUDO Hub):**
   - إيداع إلزامي للسلع المادية لدى إحدى المكتبات المعتمدة داخل الحرم.
   - مراجعة وفحص السلعة واعتمادها من قبل صاحب المكتبة قبل ظهورها في السوق.
   - شارة توثيق المكتبة المستلمة على بطاقة السلعة لتسهيل استلام الطالب للمستلزم بأمان.

2. **الاستفسارات والتعليقات العامة المترابطة (Threaded Public Inquiries):**
   - نقاش واستفسار علني حول السلعة في صفحة التفاصيل بين الطلاب والبائع والمكتبة.
   - شارات مخصصة لصاحب الإعلان وشريك المكتبة المستلمة.
   - دعم الردود المتشعبة (Threaded Replies) وحذف التعليقات المخالفة.

3. **التوثيق الأكاديمي الرقمي (Academic Student Verification):**
   - توثيق قيد الطالب بالجامعة والكلية عبر رقم القيد الأكاديمي الرسمي.
   - منح شارة التوثيق الأزرق (Cyan Pulse `✓`) ورفع سقف الكوتا المشتركة.

4. **نظام الكوتا والصلاحيات (Quota & Dynamic Security):**
   - سقف مشترك للإعلانات النشطة وطلبات "مطلوب" (3 لغير الموثق / 5 للموثق).
   - عداد دائري تفاعلي (Quota Ring) يعكس الحصة المتبقية بدقة.

5. **تصميم Mobile-First و PWA متكامل:**
   - تصميم متجاوب موجه للهواتف الذكية مع شريط تنقل سفلي مرن (Bottom Nav Dock).
   - بطاقات شبكية عصرية (Bento-Box UI) متوافقة مع الهوية البصرية ونظام الألوان المعتمد.
   - ملف `manifest.json` وأيقونات متعددة الأحجام تتيح تثبيت الموقع كتطبيق على شاشة الجوال (PWA).

6. **لوحة تحكم وإدارة شاملة (Admin Governance Portal):**
   - لوحة مؤشرات أداء وإحصائيات حية (KPIs).
   - طابور مراجعة واعتماد توثيق الطلاب.
   - إدارة كاملة للهيكل الأكاديمي (الجامعات، الكليات، والتخصصات: إضافة، تفعيل، ربط، وفك ربط).
   - إدارة ومراقبة الإعلانات، تقارير البيع المجتمعية، وحسابات شركاء المكتبات.

---

## 🏗 البنية المعمارية (Pragmatic Clean Architecture)
المشروع مبني وفق معمارية الطبقات الأربع النظيفة مع الالتزام التام بمبدأ البساطة **(KISS Principle)**:

```text
├── src/
│   ├── Zameel.Domain/          # الكيانات (Entities)، الـ Enums، والثوابت (خالٍ من المكتبات الخارجية)
│   ├── Zameel.Application/     # الخدمات (Services)، عقود الواجهات (Interfaces)، و الـ DTOs
│   ├── Zameel.Infrastructure/  # Entity Framework Core، إعدادات Fluent API، وخدمات التخزين
│   └── Zameel.Web/             # وحدات التحكم (Controllers)، صفحات Razor Views، والأصول العامة
```

---

## 🛠 التقنيات المستخدمة (Tech Stack)

- **بيئة العمل:** .NET 10 (C# 14)
- **إطار الويب:** ASP.NET Core MVC (Server-Side Rendering)
- **قاعدة البيانات وتخزين البيانات:** Microsoft SQL Server عبر Entity Framework Core 10
- **واجهة المستخدم:** HTML5, CSS3 Variables, Bootstrap 5.3 (RTL), Vanilla JavaScript (No heavy SPA)
- **إدارة الملفات والأيقونات:** Multi-resolution Favicon (16/32/48/64) & WebP Image Compression

---

## 🚀 التشغيل والإعداد المحلي (Getting Started)

### المتطلبات الأساسية:
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB أو Express أو SQL Server)

### خطوات التشغيل:

1. **استنساخ المستودع (Clone the repository):**
   ```bash
   git clone https://github.com/YourUsername/Zameel.git
   cd Zameel
   ```

2. **استعادة الحزم وبناء المشروع (Restore & Build):**
   ```bash
   dotnet restore
   dotnet build
   ```

3. **تطبيق ترحيلات قاعدة البيانات (Database Migrations):**
   ```bash
   dotnet ef database update --project src/Zameel.Infrastructure --startup-project src/Zameel.Web
   ```

4. **تشغيل المشروع (Run the Application):**
   ```bash
   dotnet run --project src/Zameel.Web
   ```
   افتح المتصفح وتوجه إلى: `http://localhost:5000` أو الرابط الموضح في شاشة الطرفية.

---

## 👥 حسابات النظام الافتراضية (Seed Data)

يتم تزويد قاعدة البيانات تلقائياً ببيانات تجريبية أولية:

| الدور (Role) | البريد الإلكتروني | الصلاحيات الافتراضية |
| :--- | :--- | :--- |
| **مدير النظام (Admin)** | `admin@zameel.app` | وصول كامل للوحة الإدارة `/Admin` |
| **شريك المكتبة (Store Partner)** | `aden.bookstore@zameel.app` | إدارة مودعات الكتب بالمكتبة `/store` |
| **طالب موثق (Verified Student)** | `ahmed.student@zameel.app` | سقف 5 إعلانات + شارة التوثيق |
| **طالبة عادية (Unverified Student)**| `sara.student@zameel.app` | سقف 3 إعلانات + إمكانية طلب التوثيق |

> **طريقة تسجيل الدخول:** يتم إدخال البريد الإلكتروني وسيصل رمز التحقق (PIN Code) المكون من 4 أرقام لتأكيد الدخول الآمن.

---

## 📄 الترخيص (License)
هذا المشروع متاح للاستخدام الأكاديمي والتطوير وفق معايير المصدر المفتوح.
