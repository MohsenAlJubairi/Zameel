---
name: mobile-first-mvc-pwa
description: Guidelines for Mobile-First ASP.NET Core MVC, Razor Views, PWA capabilities, and RTL layout.
---
# Mobile-First MVC UI, PWA & Frontend KISS Rules

1. **Server-Side Rendering First:** Razor Views, Partial Views, ViewComponents, Native `fetch` API. No SPA frameworks.
2. **Mobile-First & RTL:** RTL by default (`dir="rtl"`). Thumb-friendly touch targets (min 44px height). Bottom Navigation Bar on mobile screens.
3. **Reusable Partials:** Standardize reusable cards like `_ListingCardPartial.cshtml`.
4. **Forms:** Double validation (client + server `ModelState`), prevent double-submit by disabling buttons upon click.
5. **OpenGraph & SignalR:** OpenGraph meta tags for rich WhatsApp previews. Targeted SignalR connections only where live updates are required (chat / alert bell).
6. **PWA Manifest & Service Worker:** Fast offline fallback and installable web app experience.
