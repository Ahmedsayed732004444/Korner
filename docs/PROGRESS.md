# Progress

> Source of truth for what's next: `docs/04-IMPLEMENTATION_PLAN.md`. Work the **first unticked task**, in order.
> Tick a task in the same PR that completes it, with a one-line note (what changed, key decisions).

## M0 — Repository & engineering foundation

- [x] T0.1 — Repo structure, `.editorconfig`, `README.md`, `docs/PROGRESS.md`, `docs/adr/0001-record-architecture-decisions.md`
- [x] T0.2 — Backend solution (`Korner.sln`, `Korner.Api`, unit/integration test projects, `Directory.Build.props`, `Directory.Packages.props`)
- [x] T0.3 — Frontend app (React Router 7, pnpm, Tailwind 4, shadcn, ESLint/Prettier, Vitest, Playwright, Husky)
- [ ] T0.4 — `docker-compose.yml` (SQL Server, api, web), Dockerfiles, `.env.example`
- [ ] T0.5 — GitHub Actions (`backend.yml`, `frontend.yml`), Dependabot

## M1 — Backend foundation & authentication

- [ ] T1.1 — `Result`/`Error`/`ToProblem`/`PaginatedList`/`RequestFilters`, global exception handler, FluentValidation auto-validation
- [ ] T1.2 — Options classes + `ValidateOnStart`, `GET /config/public`, `TimeProvider`, Serilog
- [ ] T1.3 — `ApplicationDbContext`, first migration, seed runner, Hangfire (`KornerJobs` DB, `/jobs`), `HybridCache`
- [ ] T1.4 — Security headers, CORS, `OriginCheckMiddleware`, rate limit policies, maintenance-mode hook, `/health`
- [ ] T1.5 — Identity, JWT, refresh tokens, register/login/refresh/logout/confirm-email/forgot/reset, Outbox email
- [ ] T1.6 — Google OAuth (challenge, callback, link by verified email)
- [ ] T1.7 — Admin TOTP 2FA, `AuditService`
- [ ] T1.8 — `GET/PATCH /me`, `DELETE /me`, `CleanupJob` skeleton

## M2 — Frontend foundation & auth UI

- [ ] T2.1 — `app.css` theme tokens, shadcn components customised
- [ ] T2.2 — i18n (`remix-i18next`), routes registered for `ar`/`en`, `LanguageSwitcher`
- [ ] T2.3 — `env.server.ts`/`env.client.ts`, `lib/api/client.ts`, `lib/logger.ts`, `formatMoney`, `governorates.ts`, `pnpm gen:api`
- [ ] T2.4 — Layout shell (header, mobile drawer/bottom bar, footer, error boundaries, 404/500, skip-link)
- [ ] T2.5 — Auth pages (sign-in, register, confirm, forgot/reset, callback, account settings stub, route guards)
- [ ] T2.6 — `/dev/components` gallery page

## M3 — Catalogue (admin + storefront)

- [ ] T3.1 — Catalogue entities, configs, migration, soft-delete filters
- [ ] T3.1b — `IFileStorage`/`LocalFileStorage` (WebP 480/960/1440, validation, `FileUrlBuilder`)
- [ ] T3.2 — `TextNormalizer` + `ISearchService`
- [ ] T3.3 — Admin catalogue APIs (categories, brands, size guides, products, variants, images, publish)
- [ ] T3.4 — Inventory admin (low-stock filter, manual adjustment, movement history)
- [ ] T3.5 — Public catalogue APIs with `HybridCache`
- [ ] T3.6 — Storefront pages (home, category, product, search)
- [ ] T3.7 — Admin UI (`/admin` products, categories, brands, size guides, inventory, banners, content)
- [ ] T3.8 — Seed data (categories, 10 products, placeholder images)

## M4 — Cart, shipping rules & order creation

- [ ] T4.1 — Cart entities, `cart_token` cookie, cart API, merge on sign-in
- [ ] T4.2 — `ShippingRule` + 27 governorates seed, admin API/UI, `GET /shipping/governorates`
- [ ] T4.3 — `CheckoutService.QuoteAsync`
- [ ] T4.4 — Idempotency filter
- [ ] T4.5 — `POST /orders` (atomic stock reservation, order number sequence, `RequiresReview`)
- [ ] T4.6 — Cart UI (drawer, `/cart`, optimistic updates, undo, free-shipping bar)
- [ ] T4.7 — Checkout page UI
- [ ] T4.8 — Maintenance mode setting + banner

## M5 — Payments (Paymob, online only)

- [ ] T5.1 — Payment entities + migration + CHECK constraints
- [ ] T5.2 — `IPaymentProvider`, `PaymobClient`, `FakePaymentProvider`
- [ ] T5.3 — `PaymobHmac` + `PaymobTransactionParser`
- [ ] T5.4 — `PaymentService.StartAsync` + `POST /orders/{number}/payment`
- [ ] T5.5 — `PaymentProcessor` + `POST /webhooks/paymob`
- [ ] T5.6 — `GET /orders/{number}/payment-status` with throttled inquiry
- [ ] T5.7 — Hangfire jobs: `PaymentExpiryJob`, `PaymentReconciliationJob`, `RefundDispatchJob`
- [ ] T5.8 — `RefundService` + admin endpoints
- [ ] T5.9 — Frontend payment start/redirect + `/checkout/result/:number`
- [ ] T5.10 — Payment emails via Outbox
- [ ] T5.11 — Manual Paymob sandbox check (owner + Claude Code session)

## M6 — Order management, shipping, returns, account & notifications

- [ ] T6.1 — `OrderService` transition map + `POST /admin/orders/{id}/transition`
- [ ] T6.2 — Admin orders UI
- [ ] T6.3 — Customer cancel + guest tracking page
- [ ] T6.4 — Returns
- [ ] T6.5 — Account pages (orders, addresses, settings, delete account)
- [ ] T6.6 — Invoice PDF (QuestPDF)
- [ ] T6.7 — Remaining emails + alert jobs
- [ ] T6.8 — Daily report
- [ ] T6.9 — Policy pages content & footer links

## M7 — Hardening, release & go-live

- [ ] T7.1 — SEO (sitemap, robots, canonical, hreflang, JSON-LD, OG images)
- [ ] T7.2 — Performance budget
- [ ] T7.3 — Accessibility pass
- [ ] T7.4 — Security hardening (CSP, ZAP, dependency audit)
- [ ] T7.5 — E2E suite + k6 load script
- [ ] T7.6 — Analytics (GA4 + Meta Pixel, env-toggled)
- [ ] T7.7 — Hosting decision (ADR) + deploy pipeline
- [ ] T7.8 — Observability (Sentry, uptime monitor)
- [ ] T7.9 — UAT with 5 real people
- [ ] T7.10 — Go-live checklist

---

## Owner actions

_(Nothing outstanding yet. Items will be added here whenever an integration is disabled or a value from `docs/SECRETS.md` needs owner attention.)_

## Questions

_(None yet. If a spec is ambiguous or conflicts with reality, the question goes here instead of guessing.)_
