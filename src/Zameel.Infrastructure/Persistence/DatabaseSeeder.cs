using Microsoft.EntityFrameworkCore;
using Zameel.Domain.Constants;
using Zameel.Domain.Entities;
using Zameel.Domain.Enums;

namespace Zameel.Infrastructure.Persistence;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly ApplicationDbContext _context;

    public DatabaseSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // 1. Seed Universities if not exists
        var hasUniversities = await _context.universities.AnyAsync(cancellationToken);
        if (!hasUniversities)
        {
            var uniAden = new University
            {
                name = "جامعة عدن",
                city = "عدن",
                logoUrl = "/images/universities/aden.webp",
                isActive = true
            };

            var uniUst = new University
            {
                name = "جامعة العلوم والتكنولوجيا",
                city = "عدن / صنعاء",
                logoUrl = "/images/universities/ust.webp",
                isActive = true
            };

            var uniSanaa = new University
            {
                name = "جامعة صنعاء",
                city = "صنعاء",
                logoUrl = "/images/universities/sanaa.webp",
                isActive = true
            };

            var uniHadhramout = new University
            {
                name = "جامعة حضرموت",
                city = "المكلا",
                logoUrl = "/images/universities/hadhramout.webp",
                isActive = true
            };

            await _context.universities.AddRangeAsync(new[] { uniAden, uniUst, uniSanaa, uniHadhramout }, cancellationToken);

            // Seed Faculties
            var facMedicine = new Faculty { name = "كلية الطب والعلوم الصحية", slug = "medicine-and-health-sciences", isActive = true };
            var facEngineering = new Faculty { name = "كلية الهندسة", slug = "engineering", isActive = true };
            var facComputing = new Faculty { name = "كلية الحاسوب وتكنولوجيا المعلومات", slug = "computing-and-it", isActive = true };
            var facBusiness = new Faculty { name = "كلية العلوم الإدارية", slug = "business-administration", isActive = true };
            var facPharmacy = new Faculty { name = "كلية الصيدلة", slug = "pharmacy", isActive = true };
            var facLaw = new Faculty { name = "كلية الحقوق والعلوم السياسية", slug = "law-and-political-science", isActive = true };

            await _context.faculties.AddRangeAsync(new[] { facMedicine, facEngineering, facComputing, facBusiness, facPharmacy, facLaw }, cancellationToken);

            // Seed Majors
            var majorSoftwareEng = new Major { name = "هندسة البرمجيات", slug = "software-engineering", isActive = true };
            var majorComputerSci = new Major { name = "علوم الحاسوب", slug = "computer-science", isActive = true };
            var majorInfoSystems = new Major { name = "نظم المعلومات", slug = "information-systems", isActive = true };
            var majorCyberSec = new Major { name = "الأمن السيبراني", slug = "cyber-security", isActive = true };

            var majorCivilEng = new Major { name = "الهندسة المدنية", slug = "civil-engineering", isActive = true };
            var majorArchitecturalEng = new Major { name = "الهندسة المعمارية", slug = "architectural-engineering", isActive = true };
            var majorElectricalEng = new Major { name = "الهندسة الكهربائية", slug = "electrical-engineering", isActive = true };
            var majorMechanicalEng = new Major { name = "الهندسة الميكانيكية", slug = "mechanical-engineering", isActive = true };
            var majorComputerEng = new Major { name = "هندسة الحاسوب", slug = "computer-engineering", isActive = true };

            var majorGeneralMed = new Major { name = "الطب البشري والجراحة", slug = "general-medicine-surgery", isActive = true };
            var majorMedLabs = new Major { name = "المختبرات الطبية", slug = "medical-laboratories", isActive = true };
            var majorNursing = new Major { name = "التمريض العالي", slug = "nursing", isActive = true };

            var majorAccounting = new Major { name = "المحاسبة", slug = "accounting", isActive = true };
            var majorBusinessMgmt = new Major { name = "إدارة الأعمال", slug = "business-management", isActive = true };
            var majorFinanceBanking = new Major { name = "العلوم المالية والمصرفية", slug = "finance-and-banking", isActive = true };

            var majorClinicalPharmacy = new Major { name = "الصيدلة السريرية", slug = "clinical-pharmacy", isActive = true };
            var majorPublicLaw = new Major { name = "القانون العام", slug = "public-law", isActive = true };

            await _context.majors.AddRangeAsync(new[]
            {
                majorSoftwareEng, majorComputerSci, majorInfoSystems, majorCyberSec,
                majorCivilEng, majorArchitecturalEng, majorElectricalEng, majorMechanicalEng, majorComputerEng,
                majorGeneralMed, majorMedLabs, majorNursing,
                majorAccounting, majorBusinessMgmt, majorFinanceBanking,
                majorClinicalPharmacy, majorPublicLaw
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            // Link Universities with Faculties
            var uniFaculties = new List<UniversityFaculty>
            {
                new() { universityId = uniAden.id, facultyId = facMedicine.id },
                new() { universityId = uniAden.id, facultyId = facEngineering.id },
                new() { universityId = uniAden.id, facultyId = facComputing.id },
                new() { universityId = uniAden.id, facultyId = facBusiness.id },
                new() { universityId = uniAden.id, facultyId = facPharmacy.id },
                new() { universityId = uniAden.id, facultyId = facLaw.id },

                new() { universityId = uniUst.id, facultyId = facMedicine.id },
                new() { universityId = uniUst.id, facultyId = facEngineering.id },
                new() { universityId = uniUst.id, facultyId = facComputing.id },
                new() { universityId = uniUst.id, facultyId = facBusiness.id },

                new() { universityId = uniSanaa.id, facultyId = facMedicine.id },
                new() { universityId = uniSanaa.id, facultyId = facEngineering.id },
                new() { universityId = uniSanaa.id, facultyId = facComputing.id },
                new() { universityId = uniSanaa.id, facultyId = facBusiness.id },
                new() { universityId = uniSanaa.id, facultyId = facLaw.id },

                new() { universityId = uniHadhramout.id, facultyId = facMedicine.id },
                new() { universityId = uniHadhramout.id, facultyId = facEngineering.id },
                new() { universityId = uniHadhramout.id, facultyId = facComputing.id },
                new() { universityId = uniHadhramout.id, facultyId = facBusiness.id }
            };
            await _context.universityFaculties.AddRangeAsync(uniFaculties, cancellationToken);

            // Link Faculties with Majors
            var facultyMajors = new List<FacultyMajor>
            {
                new() { facultyId = facComputing.id, majorId = majorSoftwareEng.id },
                new() { facultyId = facComputing.id, majorId = majorComputerSci.id },
                new() { facultyId = facComputing.id, majorId = majorInfoSystems.id },
                new() { facultyId = facComputing.id, majorId = majorCyberSec.id },

                new() { facultyId = facEngineering.id, majorId = majorComputerEng.id },
                new() { facultyId = facEngineering.id, majorId = majorCivilEng.id },
                new() { facultyId = facEngineering.id, majorId = majorArchitecturalEng.id },
                new() { facultyId = facEngineering.id, majorId = majorElectricalEng.id },
                new() { facultyId = facEngineering.id, majorId = majorMechanicalEng.id },

                new() { facultyId = facMedicine.id, majorId = majorGeneralMed.id },
                new() { facultyId = facMedicine.id, majorId = majorMedLabs.id },
                new() { facultyId = facMedicine.id, majorId = majorNursing.id },

                new() { facultyId = facBusiness.id, majorId = majorAccounting.id },
                new() { facultyId = facBusiness.id, majorId = majorBusinessMgmt.id },
                new() { facultyId = facBusiness.id, majorId = majorFinanceBanking.id },

                new() { facultyId = facPharmacy.id, majorId = majorClinicalPharmacy.id },
                new() { facultyId = facLaw.id, majorId = majorPublicLaw.id }
            };
            await _context.facultyMajors.AddRangeAsync(facultyMajors, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        // 2. Seed Permissions
        var allPermissions = new List<Permission>
        {
            new() { systemName = SystemPermissions.ListingsApprove, displayName = "الموافقة على الإعلانات والطلبات", module = "الإعلانات", description = "مراجعة واعتماد إعلانات الطلاب وطلبات الشراء المعلقة قبل النشر" },
            new() { systemName = SystemPermissions.ListingsDelete, displayName = "حذف الإعلانات المخالفة", module = "الإعلانات", description = "حذف أي إعلان أو محتوى مخالف في المنصة" },
            new() { systemName = SystemPermissions.ListingsManage, displayName = "إدارة الإعلانات", module = "الإعلانات", description = "تعديل وإعادة نشر وتثبيت الإعلانات" },
            new() { systemName = SystemPermissions.UsersManage, displayName = "إدارة المستخدمين", module = "المستخدمين", description = "استعراض وتعديل حسابات المستخدمين ورتبهم" },
            new() { systemName = SystemPermissions.UsersVerify, displayName = "توثيق القيد الجامعي للطلاب", module = "المستخدمين", description = "مراجعة واعتماد وتأكيد أرقام القيد الجامعي للطلاب" },
            new() { systemName = SystemPermissions.StoresManage, displayName = "إدارة المتاجر والشركاء", module = "المتاجر", description = "اعتماد نقاط الإيداع والاستلام وتجميد حسابات المتاجر" },
            new() { systemName = SystemPermissions.RolesManage, displayName = "إدارة الأدوار والصلاحيات", module = "الأمان والـ RBAC", description = "إنشاء وتعديل الأدوار وربط الصلاحيات بها وتعيينها للمستخدمين" },
            new() { systemName = SystemPermissions.PlansManage, displayName = "إدارة باقات الاشتراك", module = "الباقات", description = "إنشاء وتعديل باقات الطلاب والمتاجر وتحديد سقوف الكوتا" },
            new() { systemName = SystemPermissions.CatalogManage, displayName = "إدارة الدليل الأكاديمي", module = "الدليل الأكاديمي", description = "إضافة وتعديل الجامعات والكليات والتخصصات وربطها" }
        };

        foreach (var p in allPermissions)
        {
            var exists = await _context.permissions.AnyAsync(x => x.systemName == p.systemName, cancellationToken);
            if (!exists)
            {
                await _context.permissions.AddAsync(p, cancellationToken);
            }
        }
        await _context.SaveChangesAsync(cancellationToken);

        var dbPermissions = await _context.permissions.ToListAsync(cancellationToken);

        // 3. Seed Roles
        var roleAdmin = await _context.roles.FirstOrDefaultAsync(r => r.normalizedName == SystemRoles.Admin, cancellationToken);
        if (roleAdmin == null)
        {
            roleAdmin = new Role
            {
                name = "مدير النظام",
                normalizedName = SystemRoles.Admin,
                description = "كامل صلاحيات الإدارة والتحكم في منصة زميل",
                isSystemRole = true
            };
            await _context.roles.AddAsync(roleAdmin, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var roleModerator = await _context.roles.FirstOrDefaultAsync(r => r.normalizedName == SystemRoles.Moderator, cancellationToken);
        if (roleModerator == null)
        {
            roleModerator = new Role
            {
                name = "مشرف المنصة",
                normalizedName = SystemRoles.Moderator,
                description = "مراجعة واعتماد الإعلانات وتوثيق الطلاب",
                isSystemRole = true
            };
            await _context.roles.AddAsync(roleModerator, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var roleStore = await _context.roles.FirstOrDefaultAsync(r => r.normalizedName == SystemRoles.Store, cancellationToken);
        if (roleStore == null)
        {
            roleStore = new Role
            {
                name = "مكتبة / شريك معتمد",
                normalizedName = SystemRoles.Store,
                description = "حساب تجاري للمكتبات الجامعية ونقاط استلام المستلزمات",
                isSystemRole = true
            };
            await _context.roles.AddAsync(roleStore, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var roleStudentVerified = await _context.roles.FirstOrDefaultAsync(r => r.normalizedName == SystemRoles.StudentVerified, cancellationToken);
        if (roleStudentVerified == null)
        {
            roleStudentVerified = new Role
            {
                name = "طالب موثق",
                normalizedName = SystemRoles.StudentVerified,
                description = "طالب جامعي موثق برقم القيد الجامعي (سقف 5 إعلانات)",
                isSystemRole = true
            };
            await _context.roles.AddAsync(roleStudentVerified, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var roleStudentUnverified = await _context.roles.FirstOrDefaultAsync(r => r.normalizedName == SystemRoles.StudentUnverified, cancellationToken);
        if (roleStudentUnverified == null)
        {
            roleStudentUnverified = new Role
            {
                name = "طالب عادي",
                normalizedName = SystemRoles.StudentUnverified,
                description = "طالب جديد غير موثق (سقف 3 إعلانات)",
                isSystemRole = true
            };
            await _context.roles.AddAsync(roleStudentUnverified, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // 4. Link Role Permissions
        // Admin gets all permissions
        foreach (var p in dbPermissions)
        {
            var exists = await _context.rolePermissions.AnyAsync(rp => rp.roleId == roleAdmin.id && rp.permissionId == p.id, cancellationToken);
            if (!exists)
            {
                await _context.rolePermissions.AddAsync(new RolePermission { roleId = roleAdmin.id, permissionId = p.id }, cancellationToken);
            }
        }

        // Moderator gets ListingsApprove, ListingsDelete, UsersVerify
        var modPermissions = dbPermissions.Where(p =>
            p.systemName == SystemPermissions.ListingsApprove ||
            p.systemName == SystemPermissions.ListingsDelete ||
            p.systemName == SystemPermissions.UsersVerify).ToList();

        foreach (var p in modPermissions)
        {
            var exists = await _context.rolePermissions.AnyAsync(rp => rp.roleId == roleModerator.id && rp.permissionId == p.id, cancellationToken);
            if (!exists)
            {
                await _context.rolePermissions.AddAsync(new RolePermission { roleId = roleModerator.id, permissionId = p.id }, cancellationToken);
            }
        }

        // Store gets ListingsManage, StoresManage
        var storePermissions = dbPermissions.Where(p =>
            p.systemName == SystemPermissions.ListingsManage ||
            p.systemName == SystemPermissions.StoresManage).ToList();

        foreach (var p in storePermissions)
        {
            var exists = await _context.rolePermissions.AnyAsync(rp => rp.roleId == roleStore.id && rp.permissionId == p.id, cancellationToken);
            if (!exists)
            {
                await _context.rolePermissions.AddAsync(new RolePermission { roleId = roleStore.id, permissionId = p.id }, cancellationToken);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // 5. Seed Plans / Packages (الباقات)
        var planBasic = await _context.plans.FirstOrDefaultAsync(p => p.isDefault, cancellationToken);
        if (planBasic == null)
        {
            planBasic = new Plan
            {
                name = "الباقة الأساسية المجانية",
                description = "الباقة الافتراضية للطلاب الجدد مع كوتا 3 إعلانات نشطة مجاناً",
                price = 0m,
                maxActiveListings = 3,
                durationDays = 365,
                hasVerifiedBadge = false,
                hasStoreProfile = false,
                isDefault = true,
                isActive = true
            };
            await _context.plans.AddAsync(planBasic, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var planVerified = await _context.plans.FirstOrDefaultAsync(p => p.hasVerifiedBadge && !p.hasStoreProfile, cancellationToken);
        if (planVerified == null)
        {
            planVerified = new Plan
            {
                name = "باقة الطالب الموثق",
                description = "مزايا إضافية لطلاب الجامعات الموثقين مع شارة التوثيق وكوتا 5 إعلانات نشطة",
                price = 0m,
                maxActiveListings = 5,
                durationDays = 365,
                hasVerifiedBadge = true,
                hasStoreProfile = false,
                isDefault = false,
                isActive = true
            };
            await _context.plans.AddAsync(planVerified, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var planStore = await _context.plans.FirstOrDefaultAsync(p => p.hasStoreProfile, cancellationToken);
        if (planStore == null)
        {
            planStore = new Plan
            {
                name = "باقة الشريك والمكتبات الجامعية",
                description = "حساب تجاري متكامل للمكتبات ونقاط الاستلام مع ملف متجر مخصص وكوتا 50 إعلان نشط",
                price = 15000m,
                maxActiveListings = 50,
                durationDays = 365,
                hasVerifiedBadge = true,
                hasStoreProfile = true,
                isDefault = false,
                isActive = true
            };
            await _context.plans.AddAsync(planStore, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // 6. Load primary catalog references
        var aden = await _context.universities.FirstOrDefaultAsync(u => u.name.Contains("عدن"), cancellationToken)
                   ?? await _context.universities.FirstAsync(cancellationToken);

        var computingFaculty = await _context.faculties.FirstOrDefaultAsync(f => f.slug.Contains("computing"), cancellationToken);
        var engineeringFaculty = await _context.faculties.FirstOrDefaultAsync(f => f.slug.Contains("engineering"), cancellationToken);
        var medicineFaculty = await _context.faculties.FirstOrDefaultAsync(f => f.slug.Contains("medicine"), cancellationToken);

        var softwareMajor = await _context.majors.FirstOrDefaultAsync(m => m.slug.Contains("software"), cancellationToken);
        var medMajor = await _context.majors.FirstOrDefaultAsync(m => m.slug.Contains("medicine"), cancellationToken);
        var compEngMajor = await _context.majors.FirstOrDefaultAsync(m => m.slug.Contains("computer-engineering"), cancellationToken);

        // 7. Seed Demo Users
        var adminUser = await _context.users.FirstOrDefaultAsync(u => u.email == "admin@zameel.app", cancellationToken);
        if (adminUser == null)
        {
            adminUser = new User
            {
                email = "admin@zameel.app",
                fullName = "مدير نظام زميل",
                roleId = roleAdmin.id,
                planId = planStore.id,
                isVerified = true,
                verifiedAt = DateTime.UtcNow
            };
            await _context.users.AddAsync(adminUser, cancellationToken);
        }
        else
        {
            adminUser.roleId = roleAdmin.id;
            adminUser.planId = planStore.id;
        }

        var moderatorUser = await _context.users.FirstOrDefaultAsync(u => u.email == "moderator@zameel.app", cancellationToken);
        if (moderatorUser == null)
        {
            moderatorUser = new User
            {
                email = "moderator@zameel.app",
                phone = "967770000004",
                fullName = "مشرف الاعتماد والمراجعة",
                roleId = roleModerator.id,
                planId = planVerified.id,
                universityId = aden.id,
                facultyId = computingFaculty?.id,
                isVerified = true,
                verifiedAt = DateTime.UtcNow
            };
            await _context.users.AddAsync(moderatorUser, cancellationToken);
        }
        else
        {
            moderatorUser.roleId = roleModerator.id;
            moderatorUser.planId = planVerified.id;
        }

        var verifiedStudent = await _context.users.FirstOrDefaultAsync(u => u.email == "ahmed.student@zameel.app", cancellationToken);
        if (verifiedStudent == null)
        {
            verifiedStudent = new User
            {
                email = "ahmed.student@zameel.app",
                phone = "967770000001",
                fullName = "أحمد محمد (طالب موثق)",
                roleId = roleStudentVerified.id,
                planId = planVerified.id,
                universityId = aden.id,
                facultyId = computingFaculty?.id,
                majorId = softwareMajor?.id,
                studentRegNo = "20210089",
                isVerified = true,
                verifiedAt = DateTime.UtcNow,
                activeListingsCount = 0
            };
            await _context.users.AddAsync(verifiedStudent, cancellationToken);
        }
        else
        {
            verifiedStudent.roleId = roleStudentVerified.id;
            verifiedStudent.planId = planVerified.id;
            if (string.IsNullOrEmpty(verifiedStudent.studentRegNo))
                verifiedStudent.studentRegNo = "20210089";
        }

        var unverifiedStudent = await _context.users.FirstOrDefaultAsync(u => u.email == "sara.student@zameel.app", cancellationToken);
        if (unverifiedStudent == null)
        {
            unverifiedStudent = new User
            {
                email = "sara.student@zameel.app",
                phone = "967770000002",
                fullName = "سارة علي (طالبة جديدة)",
                roleId = roleStudentUnverified.id,
                planId = planBasic.id,
                universityId = aden.id,
                facultyId = medicineFaculty?.id,
                majorId = medMajor?.id,
                studentRegNo = "20230145",
                isVerified = false,
                activeListingsCount = 0
            };
            await _context.users.AddAsync(unverifiedStudent, cancellationToken);
        }
        else
        {
            unverifiedStudent.roleId = roleStudentUnverified.id;
            unverifiedStudent.planId = planBasic.id;
            if (string.IsNullOrEmpty(unverifiedStudent.studentRegNo))
                unverifiedStudent.studentRegNo = "20230145";
        }

        var storeUser = await _context.users.FirstOrDefaultAsync(u => u.email == "aden.bookstore@zameel.app", cancellationToken);
        if (storeUser == null)
        {
            storeUser = new User
            {
                email = "aden.bookstore@zameel.app",
                phone = "967770000003",
                fullName = "مكتبة الزميل الجامعية",
                roleId = roleStore.id,
                planId = planStore.id,
                universityId = aden.id,
                facultyId = engineeringFaculty?.id,
                isVerified = true,
                verifiedAt = DateTime.UtcNow
            };
            await _context.users.AddAsync(storeUser, cancellationToken);

            var storeProfile = new StoreProfile
            {
                userId = storeUser.id,
                storeName = "قرطاسية وخدمات الطالب الجامعي",
                description = "تصوير الملازم الجامعية، بيع الأدوات الهندسية والطبية، ومقررات السنة الأولى.",
                locationDescription = "أمام بوابة كلية الهندسة الرئيسية",
                subscriptionExpiresAt = DateTime.UtcNow.AddYears(1),
                isApproved = true
            };
            await _context.storeProfiles.AddAsync(storeProfile, cancellationToken);
        }
        else
        {
            storeUser.roleId = roleStore.id;
            storeUser.planId = planStore.id;
        }

        await _context.SaveChangesAsync(cancellationToken);

        // 8. Seed Realistic Campus Listings if none exist
        var hasListings = await _context.listings.AnyAsync(cancellationToken);
        if (!hasListings)
        {
            var storeProfileRef = await _context.storeProfiles.FirstOrDefaultAsync(s => s.userId == storeUser.id, cancellationToken);

            var listing1 = new Listing
            {
                userId = verifiedStudent.id,
                universityId = aden.id,
                primaryFacultyId = computingFaculty?.id,
                primaryMajorId = softwareMajor?.id,
                title = "كتاب هندسة البرمجيات Software Engineering (الطبعة العاشرة - Ian Sommerville)",
                description = "الكتاب المعتمد لمقرر هندسة البرمجيات 1 و 2 بكلية الحاسوب. نظيف جداً وخالٍ من الخدوش مع كامل الملخصات المترجمة للسنة الثالثة.",
                price = 5000m,
                type = ListingType.ForSale,
                condition = ItemCondition.LikeNew,
                scope = ListingScope.MajorSpecific,
                status = ListingStatus.Active,
                pickupStoreId = storeProfileRef?.id,
                expiresAt = DateTime.UtcNow.AddDays(30),
                images = new List<ListingImage>
                {
                    new() { imageUrl = "/images/items/software_book.webp", displayOrder = 1, isPrimary = true }
                }
            };

            var listing2 = new Listing
            {
                userId = unverifiedStudent.id,
                universityId = aden.id,
                primaryFacultyId = medicineFaculty?.id,
                primaryMajorId = medMajor?.id,
                title = "سماعة طبية 3M Littmann Classic III (أصلية بحالة ممتازة)",
                description = "سماعة ليتمان كلاسيك 3 أصلية مستخدمة لفصل دراسي واحد فقط للتدريب السريري، الصوت نقي مع غيارات الأذن الأصلية والكرتون.",
                price = 25000m,
                type = ListingType.ForSale,
                condition = ItemCondition.LikeNew,
                scope = ListingScope.FacultySpecific,
                status = ListingStatus.Active,
                pickupStoreId = storeProfileRef?.id,
                expiresAt = DateTime.UtcNow.AddDays(30),
                images = new List<ListingImage>
                {
                    new() { imageUrl = "/images/items/littmann.webp", displayOrder = 1, isPrimary = true }
                }
            };

            var listing3 = new Listing
            {
                userId = verifiedStudent.id,
                universityId = aden.id,
                primaryFacultyId = engineeringFaculty?.id,
                primaryMajorId = compEngMajor?.id,
                title = "آلة حاسبة علمية متطورة Casio fx-991EX ClassWiz",
                description = "الآلة المعتمدة لطلاب الهندسة والحاسوب لحل المعادلات والتكامل والمصفوفات بسرعة. تعمل بالطاقة الشمسية والبطارية.",
                price = 8000m,
                type = ListingType.ForSale,
                condition = ItemCondition.Used,
                scope = ListingScope.FacultySpecific,
                status = ListingStatus.Active,
                pickupStoreId = storeProfileRef?.id,
                expiresAt = DateTime.UtcNow.AddDays(30),
                images = new List<ListingImage>
                {
                    new() { imageUrl = "/images/items/casio.webp", displayOrder = 1, isPrimary = true }
                }
            };

            var listing4 = new Listing
            {
                userId = unverifiedStudent.id,
                universityId = aden.id,
                primaryFacultyId = medicineFaculty?.id,
                primaryMajorId = medMajor?.id,
                title = "دفاتر وملخصات التشريح البشري والأنسجة (مطبوعة ومغلفة) 🎁",
                description = "ملخصات شاملة لمادة التشريح والفيزيولوجيا للسنة الأولى طب بشري وصيدلة مع أسئلة الامتحانات السابقة مجاناً كإهداء لأي زميل مستجد.",
                price = 0m,
                type = ListingType.Donation,
                condition = ItemCondition.Used,
                scope = ListingScope.CampusWide,
                status = ListingStatus.Active,
                pickupStoreId = storeProfileRef?.id,
                expiresAt = DateTime.UtcNow.AddDays(30),
                images = new List<ListingImage>
                {
                    new() { imageUrl = "/images/items/notes.webp", displayOrder = 1, isPrimary = true }
                }
            };

            var listing5 = new Listing
            {
                userId = storeUser.id,
                universityId = aden.id,
                primaryFacultyId = engineeringFaculty?.id,
                title = "طقم أدوات الرسم الهندسي ومسطرة T-Square أصلية 80cm",
                description = "متوفر لدى مكتبة الزميل: طقم مثلثات وفرجار هندسي ومسطرة حرف تي روترنج مع شنطة حفظ لطلاب السنة الإعدادية كلية الهندسة.",
                price = 6000m,
                type = ListingType.ForSale,
                condition = ItemCondition.New,
                scope = ListingScope.FacultySpecific,
                status = ListingStatus.Active,
                pickupStoreId = storeProfileRef?.id,
                expiresAt = DateTime.UtcNow.AddDays(60),
                images = new List<ListingImage>
                {
                    new() { imageUrl = "/images/items/drawing_set.webp", displayOrder = 1, isPrimary = true }
                }
            };

            var listing6Reported = new Listing
            {
                userId = verifiedStudent.id,
                universityId = aden.id,
                primaryFacultyId = computingFaculty?.id,
                title = "راوتر واي فاي محمول 4G بطارية تدوم 8 ساعات",
                description = "جهاز ماي فاي محمول مناسب للدراسة في السكن الجامعي والمكتبة.",
                price = 12000m,
                type = ListingType.ForSale,
                condition = ItemCondition.Used,
                scope = ListingScope.CampusWide,
                status = ListingStatus.Active,
                pickupStoreId = storeProfileRef?.id,
                reportedSoldCount = 2,
                expiresAt = DateTime.UtcNow.AddDays(15),
                images = new List<ListingImage>
                {
                    new() { imageUrl = "/images/items/router.webp", displayOrder = 1, isPrimary = true }
                }
            };

            // Listing awaiting moderator/admin approval for demonstration!
            var listing7Pending = new Listing
            {
                userId = unverifiedStudent.id,
                universityId = aden.id,
                primaryFacultyId = medicineFaculty?.id,
                primaryMajorId = medMajor?.id,
                title = "ميكروسكوب ضوئي مخبري مخصص لطلاب الطب البشري",
                description = "ميكروسكوب نظيف جداً بقوة تكبير 1000x مناسب لفحص شرائح علم الأنسجة والميكروبيولوجي، بانتظار مراجعة واعتماد المشرف.",
                price = 45000m,
                type = ListingType.ForSale,
                condition = ItemCondition.LikeNew,
                scope = ListingScope.FacultySpecific,
                status = ListingStatus.PendingApproval,
                pickupStoreId = storeProfileRef?.id,
                expiresAt = DateTime.UtcNow.AddDays(30),
                images = new List<ListingImage>
                {
                    new() { imageUrl = "/images/items/microscope.webp", displayOrder = 1, isPrimary = true }
                }
            };

            await _context.listings.AddRangeAsync(new[] { listing1, listing2, listing3, listing4, listing5, listing6Reported, listing7Pending }, cancellationToken);

            // Update user active listing counts
            verifiedStudent.activeListingsCount = 3;
            unverifiedStudent.activeListingsCount = 2;
            storeUser.activeListingsCount = 1;

            await _context.SaveChangesAsync(cancellationToken);

            // Seed a Community "Sold" Report so Admin has report data
            var soldReport = new ListingReport
            {
                listingId = listing6Reported.id,
                reporterId = unverifiedStudent.id,
                reason = ReportReason.ItemSold,
                notes = "تم التواصل مع البائع وأفاد بأنه تم بيع الراوتر بالفعل."
            };
            await _context.listingReports.AddAsync(soldReport, cancellationToken);

            // Seed Sample Campus Comments & Inquiries
            var comment1 = new Comment
            {
                listingId = listing1.id,
                userId = unverifiedStudent.id,
                content = "السلام عليكم يا زميلي، هل كتاب هندسة البرمجيات ما زال متوفراً في مكتبة الكلية؟",
                createdAt = DateTime.UtcNow.AddHours(-3)
            };
            await _context.comments.AddAsync(comment1, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var reply1 = new Comment
            {
                listingId = listing1.id,
                userId = verifiedStudent.id,
                parentCommentId = comment1.id,
                content = "وعليكم السلام ورحمة الله! نعم يا سارة، الكتاب مودع وجاهز للمعاينة والاستلام في مكتبة الطالب الجامعي.",
                createdAt = DateTime.UtcNow.AddHours(-2)
            };
            await _context.comments.AddAsync(reply1, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
