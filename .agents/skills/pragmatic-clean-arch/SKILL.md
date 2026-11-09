---
name: pragmatic-clean-arch
description: Guidelines for 4-layer Pragmatic Clean Architecture in .NET adhering strictly to the KISS principle.
---
# Pragmatic Clean Architecture & KISS Principle

## Structure:
1. `Zameel.Domain`: Pure POCO Entities, Enums, Constants. Zero external dependencies.
2. `Zameel.Application`: Direct Services (`AuthService`, `ListingService`, `QuotaService`), DTOs, Manual Object Mapping, `IApplicationDbContext`. No MediatR, No CQRS, No AutoMapper, No Generic Repository.
3. `Zameel.Infrastructure`: EF Core `ApplicationDbContext`, Fluent API Configurations, `IApplicationDbContext` implementation, File Storage (WebP), Background Services.
4. `Zameel.Web`: ASP.NET Core MVC Controllers, Razor Views, Partial Views, SignalR Hubs. No business rules in controllers or views.
