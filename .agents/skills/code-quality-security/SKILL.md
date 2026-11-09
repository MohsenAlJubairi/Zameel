---
name: code-quality-security
description: Code quality rules, Guard Clauses, ServiceResult pattern, RBAC and Quota separation, and security measures.
---
# Code Quality, Error Handling & Dynamic Security

1. **Guard Clauses:** Early returns on failures and validations to keep code flat.
2. **Unified `ServiceResult<T>`:** Never throw exceptions for expected business errors. Return `ServiceResult<T>` with `isSuccess`, `message`, `data`.
3. **RBAC & Quotas Separation:**
   - Permissions check: "Can user execute this action?" (`[HasPermission]`).
   - Quotas check: "Does current active limit allow this action?" (`QuotaService.CanCreateListingAsync`).
   - Shared quota pool: Active ForSale/Donation/Bundle + Requests <= Quota Limit (3 unverified, 5 verified).
4. **Security & Files:**
   - Guid file naming with validation of extension and MIME.
   - Compress and convert images to `WebP` automatically.
   - Rate limiting on OTP and messaging actions.
