---
name: centralization-and-conventions
description: Centralization of design tokens, system alerts, messages, and enforcement of camelCase naming conventions.
---
# Centralization and Naming Conventions

1. **Centralized Styling & Colors:**
   - Palette colors, font families, radius, and shadows defined in `:root` CSS variables in `site.css`.
   - Components inherit and use `var(--...)` for consistency.
2. **Centralized Messages & Alerts:**
   - Single point of truth for all alert, toast, validation, and error messages (`AppMessages.cs` / `SystemAlerts.cs`).
   - No hardcoded strings scattered in controllers or views.
3. **camelCase Convention:**
   - All DB tables and column names configured in EF Core as `camelCase` (e.g. `listingId`, `userId`, `createdAt`, `universityId`, `facultyName`).
   - All JSON serialization configured to `camelCase`.
