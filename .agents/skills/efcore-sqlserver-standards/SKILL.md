---
name: efcore-sqlserver-standards
description: EF Core and SQL Server standards focusing on Fluent API, camelCase naming, Restrict delete behavior, and performance.
---
# EF Core & SQL Server Coding Standards

1. **Fluent API Only:** All configurations in `Infrastructure` using `IEntityTypeConfiguration<T>`. Pure POCOs in `Domain`.
2. **DeleteBehavior.Restrict:** No cascade delete on core relationships to avoid cycles in SQL Server.
3. **Soft Delete Filter:** Global query filter `e => !e.isDeleted` on all `BaseEntity` subclasses.
4. **Performance:**
   - `.AsNoTracking()` for read queries.
   - `.Select()` direct projection.
   - Paging with `.Skip().Take()`.
   - Explicit composite indexes for frequent search paths (`universityId`, `facultyId`, `status`, `createdAt`).
5. **camelCase Convention:** Tables and columns are mapped to camelCase names (e.g. `listingId`, `userId`, `price`).
