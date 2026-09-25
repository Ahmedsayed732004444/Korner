# Korner — Implementation Plan (SDLC)

> Build order for Claude Code. Work **one milestone at a time, one task at a time**, in order.
> Each task lists what to build, the requirement IDs it satisfies, and how it is accepted.
> Specs: `01-PRODUCT_SPEC.md` (what), `02-TECHNICAL_DESIGN.md` (how), `03-DESIGN_SYSTEM.md` (look & UX).
> Repository: `https://github.com/Ahmedsayed732004444/Korner.git` (monorepo: `backend/`, `frontend/`, `docs/`).

---

## 0. The lifecycle we follow

| # | SDLC phase | In this project | Output |
| --- | --- | --- | --- |
| 1 | Planning & requirements | Done by the owner with Claude | `01-PRODUCT_SPEC.md` |
| 2 | System design | Done; refined in ADRs while building | `02-TECHNICAL_DESIGN.md`, `03-DESIGN_SYSTEM.md`, `docs/adr/` |
| 3 | Environment & foundations | Milestone **M0** | Repo, tooling, CI, local stack |
| 4 | Implementation (iterative, vertical slices) | Milestones **M1 → M6**; every slice = backend + frontend + tests | Working features behind green CI |
| 5 | Verification & validation | Continuous tests in every task + milestone **M7** hardening, UAT | Test reports, Lighthouse/axe, security scan |
| 6 | Deployment | Milestone **M7** (staging → production) | Live store |
| 7 | Operation & maintenance | Section 10 | Monitoring, backups, P2 backlog |

**Rules that apply to every task (Definition of Done):**

1. Code follows `02-TECHNICAL_DESIGN.md` §2 cross-cutting rules and the folder structures in §3.1 / §9.1.
2. Tests written in the same PR: unit tests for logic, integration tests for anything touching money, stock, auth or payments, component tests for UI states.
3. `backend`: `dotnet build` with 0 warnings, all tests green. `frontend`: `pnpm lint && pnpm typecheck && pnpm test` green.
4. UI works at 360 px and 1280 px, in Arabic (RTL) and English (LTR), with keyboard only; all states from design system §7.
5. No secrets committed or printed (see `docs/SECRETS.md`), no `console.*`, no physical CSS direction classes, no hard-coded UI strings (use i18n keys in `ar` and `en`).
6. New error codes added to `lib/i18n/locales/{ar,en}/errors.json`.
7. EF migration added when the model changes (name: `<Milestone>_<Change>`), and it is backward compatible.
8. Every response a UI builds actions from carries server-computed permission flags (`isMine`, `canEdit`, `canCancel`, `allowedTransitions`, …) produced by the same policy the action enforces, with a test (technical design §2.10).
9. `docs/PROGRESS.md` updated (task ticked + short note), Conventional Commit message referencing task and requirement IDs, e.g. `feat(checkout): create order with atomic stock reservation [T4.5, CHK-08]`.

---

## 1. Owner prerequisites (not Claude Code tasks)

| Needed by | Item |
| --- | --- |
| M0 | GitHub repo access for Claude Code (`gh auth login`), branch protection on `main` |
| M1 | ✅ Google OAuth Client ID + Secret provided (in `appsettings.Local.json`). Owner must add the authorised redirect URI `https://localhost:5081/signin-google` in Google Cloud Console (later `https://api.{domain}/signin-google`) |
| M1 | ✅ Gmail SMTP app password provided (`MailSettings`). Before launch: a domain mailbox with SPF, DKIM, DMARC (T7.10) |
| M5 | ✅ Paymob **test** keys + card integration ID provided. ❌ Wallet integration ID is `0` (not activated) — create a Mobile Wallet integration and send its ID to enable wallets. Confirm the transaction-inquiry path in the dashboard API explorer |
| M5 | A tunnel for webhooks in dev (cloudflared or ngrok) |
| M7 | Domain name (e.g. korner.com), hosting decision (D13), Paymob **live** keys, Sentry DSNs, uptime monitor, logo files (SVG, light & dark) |
| M7 | Real content: product photos & data, policy page texts, 27 governorate shipping fees/days, free-shipping threshold |

---

## 2. How Claude Code works in this repo (session protocol)

1. Read `CLAUDE.md`, then the milestone section below, then only the spec sections the task references.
2. Open `docs/PROGRESS.md`, take the **first unticked task** of the current milestone. Never start the next milestone before the exit criteria of the current one are met.
3. Create a branch `feat/m{n}-{short-name}` from `main`.
4. Write a short plan (files to add/change, tests) in the PR description before coding.
5. Implement with tests; run the full checks for the part you touched.
6. Update `docs/PROGRESS.md`; commit; push; open a PR with `gh pr create` including: what was done, requirement IDs, how it was tested, screenshots for UI (360 px + 1280 px, ar + en).
7. If the spec is ambiguous or conflicts with reality (e.g. a Paymob field differs), stop and write the question in the PR / `docs/PROGRESS.md` "Questions" section instead of guessing. Record accepted design choices as ADRs in `docs/adr/NNNN-title.md`.

---

## M0 — Repository & engineering foundation

**Goal:** an empty but production-grade monorepo that builds, tests and runs locally with one command.

| Task | What to do | Acceptance |
| --- | --- | --- |
| T0.1 | Initialise repo structure from technical design §1.1; keep the provided `.gitignore` (secrets, uploads, dotnet, node) and verify `git check-ignore` for `appsettings.Local.json` and `.env` (docs/SECRETS.md rule 1); `.editorconfig`, `README.md` (setup + commands), copy these docs into `docs/`, create `docs/PROGRESS.md` listing every task of this plan as unchecked, `docs/adr/0001-record-architecture-decisions.md` | Tree matches §1.1; README commands work |
| T0.2 | Backend solution: `Korner.sln`, `src/Korner.Api` (net9.0, controllers, `launchSettings.json` with `https://localhost:5081` and `http://localhost:5080`, loads `appsettings.Local.json` in Development), `tests/Korner.UnitTests`, `tests/Korner.IntegrationTests`; `Directory.Build.props` (Nullable, ImplicitUsings, TreatWarningsAsErrors, AnalysisLevel latest); central package versions (`Directory.Packages.props`) | `dotnet build` and `dotnet test` pass on an empty test |
| T0.3 | Frontend app: React Router 7 framework template (TypeScript, SSR on), pnpm, Tailwind 4, shadcn init (`components.json`), ESLint (typescript-eslint strict, `no-console`, `import/no-restricted-paths` layer rules, rule banning `ml-*/mr-*/pl-*/pr-*/left-*/right-*` classes), Prettier, Vitest + Testing Library + MSW, Playwright, Husky + lint-staged | `pnpm lint`, `pnpm typecheck`, `pnpm test`, `pnpm build` pass |
| T0.4 | `docker-compose.yml`: SQL Server 2022 (volume, healthcheck), `api` and `web` services (dev Dockerfiles), `.env.example`; backend `Dockerfile` (multi-stage, non-root) and frontend `Dockerfile` (node server for SSR) | `docker compose up` starts all three; web shows a placeholder page calling API `/health` |
| T0.5 | GitHub Actions: `backend.yml`, `frontend.yml` as in technical design §10.4 (Lighthouse/e2e jobs added later); Dependabot for nuget + npm + actions; secret scanning note in README | CI green on the first PR |

**Exit criteria:** fresh clone → `docker compose up` works; CI green; PROGRESS.md exists.

---

## M1 — Backend foundation & authentication

**Goal:** secure API skeleton with all cross-cutting pieces and complete auth. Requirements: ACC-01, ACC-02, ACC-03, ACC-R1, ACC-R2, ACC-R3, NFR security/privacy.

| Task | What to do | Acceptance |
| --- | --- | --- |
| T1.1 | Abstractions (`Result`, `Error`, `ToProblem`, `PaginatedList`, `RequestFilters`) exactly as technical design §3.2; global exception handler → ProblemDetails 500 + Sentry; FluentValidation auto-validation returning `Validation.Failed` with error codes | Unit tests for Result invariants and ProblemDetails shape (`code`, `status`) |
| T1.2 | Options classes with DataAnnotations + `ValidateOnStart` for `Jwt`, `Auth`, `Authentication:Google`, `MailSettings`, `PaymobSettings` (section names exactly as in `appsettings.Local.json`), `HangfireSettings`, `Storage`; `GET /config/public` (technical design §2.11) with startup warnings for disabled integrations; `TimeProvider` registration; Serilog (JSON, redaction of phone/email/token fields) | App refuses to start with a missing required option (test) |
| T1.3 | `ApplicationDbContext` (partial per feature), SQL Server, `AuditLog`, `Setting`, `IdempotencyRecord`, `OutboxMessage` entities + configurations; first migration; seed runner (dev/staging only) with admin user from `Seed`; Hangfire with its own `KornerJobs` database + `/jobs` dashboard behind basic auth, following the owner's recipe (technical design §1.3, §4.4); `HybridCache` registered | Integration test boots Testcontainers SQL Server and applies migrations |
| T1.4 | Middleware: security headers, CORS from config, `OriginCheckMiddleware` (unsafe methods with cookies need allowed Origin + `X-Requested-With: korner`), rate limit policies (technical design §3.4), maintenance-mode check hook, `/health` endpoint | Integration tests: CORS rejects unknown origin; POST without header/origin → 403; limits return 429 with `RateLimit.Exceeded` |
| T1.5 | Identity (`ApplicationUser`, roles Admin/Customer), JWT provider with role claims + `amr`, refresh tokens (hashed, family rotation, reuse detection) in `korner_rt` cookie, endpoints register/login/refresh/logout/confirm-email/forgot/reset; email sending via Outbox + `OutboxJob` (Hangfire recurring + enqueue after commit) + `EmailSender` (MailKit, `MailSettings`, Gmail SMTP) with ar/en templates for confirmation & reset | Tests: login → access token with role; refresh rotates; reusing an old refresh token revokes the family; unconfirmed email cannot log in; lockout after 5 failures |
| T1.6 | Google OAuth (technical design §3.4): challenge, callback, link by verified email (ACC-03), redirect to storefront `/auth/callback` with relative `returnUrl` only | Test with a stubbed external login: new user created once; existing email linked, not duplicated |
| T1.7 | Admin TOTP 2FA: enable (QR + recovery codes), verify at login, limited token until verified, `[Authorize(Policy="AdminMfa")]` requiring role Admin + `amr=mfa`; `AuditService` + audit on security events | Admin endpoint without MFA → 403; with MFA → 200; audit rows written |
| T1.8 | Account basics: `GET/PATCH /me` (name, preferred locale), `DELETE /me` (ACC-06: disable now, `DeletionRequestedAt`), `CleanupJob` skeleton | Tests for delete flow |

**Exit criteria:** all auth flows pass integration tests; Swagger shows endpoints; no secret in repo.

---

## M2 — Frontend foundation & auth UI

**Goal:** storefront shell with theme, both languages, session handling and component library. Requirements: D5, ACC-01..03 (UI), NFR a11y/i18n.

| Task | What to do | Acceptance |
| --- | --- | --- |
| T2.1 | Paste `app.css` from design system §2; fonts via `@fontsource`; shadcn components (button, input, label, select, checkbox, radio-group, dialog, sheet, dropdown-menu, tabs, accordion, badge, toast/sonner, skeleton, table, tooltip, breadcrumb) customised with the tokens | Contrast check script (`scripts/contrast.ts` or test) passes for token pairs listed in design system §2 |
| T2.2 | i18n: `remix-i18next` server + client, namespaces (`common`, `errors`, `catalog`, `checkout`, `account`, `admin`), `routes.ts` registering public routes under `""` (ar) and `"en"`; `<html lang dir>`; `LanguageSwitcher` keeping the same page | Visiting `/` renders RTL Arabic; `/en` renders LTR English; switcher keeps path |
| T2.3 | `lib/env.server.ts` / `env.client.ts` (Zod), `lib/api/client.ts` (credentials, `X-Requested-With`, in-memory token, single-flight refresh), `lib/api/errors.ts`, `queryClient` defaults (no retry on 4xx, never retry mutations), `lib/logger.ts`, `formatMoney`, `governorates.ts` generated from `docs/data/governorates.json`; `pnpm gen:api` from backend OpenAPI | Unit tests: formatMoney ar/en; refresh single-flight with MSW; error code mapping |
| T2.4 | Layout shell: header (text wordmark "Korner", categories placeholder, search box, account, language, cart count), mobile drawer + bottom bar, footer, `ErrorBoundary` in root and routes, 404/500 pages, skip-link | Works at 360/1280 in ar/en; axe has no serious violations |
| T2.5 | Auth pages: sign-in (Google button + email/password), register, confirm email, forgot/reset, `/auth/callback` (silent refresh then redirect), account settings stub; route guards for account and admin (role + mfa) | E2E: register → confirm (from captured email in dev) → sign in → sign out; Google button starts the API challenge URL |
| T2.6 | `/dev/components` (dev only) showing every shared component in all states (design system §7) in both languages | Screenshot test of the page at 360 and 1280 |

**Exit criteria:** a user can register/sign in (email and Google) in both languages; UI shell passes axe.

---

## M3 — Catalogue (admin + storefront)

**Goal:** the owner can add real products; customers can browse, filter and search. Requirements: CAT-01..CAT-12, ADM-01, ADM-02, ADM-03 (stock view & manual adjustments), ADM-07 (banners & pages), NFR SEO/performance.

| Task | What to do | Acceptance |
| --- | --- | --- |
| T3.1 | Entities + configs + migration: Category, Brand, SizeGuide, Product, ProductImage (relative `Path`), ProductVariant (rowversion), StockMove, Banner, ContentPage; soft-delete filters; unique indexes | Migration applies; unique constraints tested |
| T3.1b | `IFileStorage` + `LocalFileStorage` from the owner's `FileHelper` approach, improved per technical design §8 (validation by extension + magic bytes, 5 MB limit, ImageSharp WebP 480/960/1440, EXIF stripped, relative paths, `FileUrlBuilder`, safe delete after commit, static files with immutable cache), `wwwroot/uploads` Docker volume | Unit tests: rejects a renamed .exe, a 6 MB file and a corrupt image; produces 3 widths without upscaling; URL built from `Storage:PublicBaseUrl` |
| T3.2 | `TextNormalizer` + `ISearchService` (SQL implementation) per technical design §7; `SearchTextAr/En` maintained on save | Unit tests incl. the CAT-10 examples |
| T3.3 | Admin APIs: categories, brands, size guides CRUD; products CRUD (both languages), variants grid upsert (SKU unique, price > 0), publish endpoint enforcing CAT-02/CAT-03 with `Catalog.PublishIncomplete` + list of missing fields; image upload/delete/reorder endpoints (multipart, via `IFileStorage`); admin responses carry `canEdit`, `canDelete`, `canPublish` + `publishBlockers[]`; audit on price changes | Integration tests: cannot publish without variants or with a missing language; price change audited |
| T3.4 | Inventory admin: list with low-stock filter, manual adjustment with required reason (StockMove + audit), movement history | Test: sum of moves equals stock after adjustments |
| T3.5 | Public APIs: category tree, product list with filters/sort/pagination (CAT-11), product detail by slug with variants, images, size guide, delivery estimate (CAT-08), home content, content pages; `HybridCache` for these reads with key-prefix constants and `RemoveAsync` on every related admin write (owner's recipe) | Integration tests for filters, sold-out variants still returned (CAT-05), hidden products never returned |
| T3.6 | Storefront pages (SSR loaders + meta): home (banners, categories, new arrivals, best sellers, trust bar CAT-09), category (filters in URL, load more, filter bottom sheet on mobile, empty state CAT-12), product (gallery 3:4 with zoom & swipe, colour switches images CAT-04, size selector with sold-out CAT-05, size guide dialog CAT-06, perfume attributes CAT-07, delivery line CAT-08, sticky mobile buy bar), search page | Lighthouse mobile on product page meets NFR; ar/en slugs & hreflang correct; component tests for size selector |
| T3.7 | Admin UI (client-rendered, `/admin`): layout with sidebar, products list/edit (both languages side by side, variant grid, drag-and-drop multi-image upload to the API with progress, reorder and colour tagging, publish button driven by `canPublish`/`publishBlockers`), categories, brands, size guides, inventory, banners, content pages (sanitised rich text) | E2E: admin creates a product with 2 colours × 3 sizes, uploads images, publishes, sees it in the store in both languages |
| T3.8 | Seed data for dev/staging: categories, 10 products with variants, placeholder images | `docker compose up` shows a browsable store |

**Exit criteria:** owner adds a real product end-to-end; category/product pages pass Lighthouse & axe gates.

---

## M4 — Cart, shipping rules & order creation

**Goal:** customer builds a cart and creates an order that reserves stock safely. Requirements: CART-01..05, CHK-01..05, CHK-07, CHK-08, SHP-01, SHP-02 (calculation), ADM-06, ADM-09, ACC-04.

| Task | What to do | Acceptance |
| --- | --- | --- |
| T4.1 | Cart entities + `cart_token` cookie issuance, cart API (get/add/update/remove, quantity 1–10, cap by stock for InStock), merge on sign-in (CART-02) | Integration tests: persistence by cookie; merge sums quantities without duplicates |
| T4.2 | ShippingRule entity + seed of 27 governorates (inactive by default in production seed), admin API + UI to edit fee/days/active and the free-shipping threshold (ADM-06); `GET /shipping/governorates` | Inactive governorate not returned; admin edit audited |
| T4.3 | `CheckoutService.QuoteAsync` (technical design §3.6) incl. free shipping, expected delivery date (SHP-02 incl. dropship lead time), `changes[]` detection (CART-05) | Unit tests for totals & dates; integration test for price-changed detection |
| T4.4 | Idempotency filter (`IdempotencyRecord`, scope + key + request hash, 24 h) | Tests: replay returns same response; different body → `409 Idempotency.Mismatch` |
| T4.5 | `POST /orders` (technical design §3.6): validation (CHK-02/03/07), maintenance mode (ADM-09), atomic stock reservation, order number sequence, snapshot items, `RequiresReview` rule, clear cart, link to user or guest token; ACC-04 linking of guest orders on registration/confirmation by email | **Integration test: 20 concurrent orders on stock 1 → exactly 1 succeeds, stock never negative**; idempotent replay; guest order appears in "My orders" after registering with same email |
| T4.6 | Cart UI: cart drawer + `/cart` page, optimistic updates with rollback, undo remove (CART-03), free-shipping bar (CART-04), price/stock change notices (CART-05) | Component tests for optimistic + rollback; E2E add/remove/undo |
| T4.7 | Checkout page UI (content order from design system §10): contact, address with governorate select (active only), live quote on change (CHK-05), payment method radio (card / wallet), consents (CHK-07), summary, Zod validation incl. phone (CHK-03), Google sign-in option (CHK-01); on submit create order with Idempotency-Key and handle `CartChanged` / `OutOfStock` | E2E guest reaches "order created" (payment step stubbed until M5); axe clean |
| T4.8 | Maintenance mode setting + admin toggle + storefront banner | Checkout blocked with message while browsing works |

**Exit criteria:** orders are created with correct totals and reserved stock under concurrency; no client-side price anywhere.

---

## M5 — Payments (Paymob, online only)

**Goal:** customers pay by card or wallet; every payment outcome is handled safely. Requirements: PAY-01..PAY-10, CHK-09, CHK-10, ORD-05 (refund part), NTF-01 (payment emails). Implement exactly technical design §4.

| Task | What to do | Acceptance |
| --- | --- | --- |
| T5.1 | Entities + migration: PaymentAttempt, PaymentTransaction, Refund; Order payment fields & CHECK constraints | Constraint test: `RefundedPiasters > PaidPiasters` rejected by SQL |
| T5.2 | `IPaymentProvider`; `PaymobClient` (typed HttpClient, root = scheme+host of `PaymobSettings:BaseUrl`, wallet disabled while `MobileIntegrationId = 0` → `Payment.MethodDisabled`, reported to owner): create intention, build checkout URL, refund, legacy auth token cache (45 min), inquiry; `FakePaymentProvider` for tests/E2E (hosted fake page posts a signed webhook) | Unit tests with fake `HttpMessageHandler`: `Token` auth header, amount in piasters, one integration id, `NA` for empty billing fields, error → `Payment.GatewayRejected`, refund outcomes Succeeded/Failed/Unknown (timeout & 5xx = Unknown) |
| T5.3 | `PaymobHmac` (20 fields, raw JSON values, SHA-512, constant-time) + `PaymobTransactionParser` | Unit test reproduces Paymob's documented string exactly; tampered amount/wrong secret/missing hmac rejected |
| T5.4 | `PaymentService.StartAsync` + `POST /orders/{number}/payment` (ownership, reuse open attempt, max 5 attempts, atomic attempt claim that extends the 15-min window) | Integration tests: double call returns same URL; 6th attempt → 429; expired reservation → 409 |
| T5.5 | `PaymentProcessor` (technical design §4.3 steps 1–8) incl. savepoint late-payment path and refund queuing; `POST /webhooks/paymob` controller | Integration tests: duplicate webhook; concurrent identical webhooks; amount mismatch → review, not confirmed; failed → retry allowed; late payment with stock → Confirmed; late payment without stock → refund queued + email queued; double payment → refund queued; refund child transaction only stored |
| T5.6 | `GET /orders/{number}/payment-status` with throttled inline inquiry | Test: webhook never sent, inquiry says paid → status `paid` |
| T5.7 | Hangfire recurring jobs: `PaymentExpiryJob`, `PaymentReconciliationJob`, `RefundDispatchJob` with `[DisableConcurrentExecution]` (technical design §4.4) | Tests with fake provider & `FakeTimeProvider`: expired unpaid order cancelled and stock released; paid-but-webhook-lost confirmed; Paymob unreachable → not cancelled until 60 min |
| T5.8 | `RefundService` (admin create with Idempotency-Key, queued execution, finalise, resolve Unknown) + admin endpoints | Tests: refund total can never exceed paid (parallel requests); Unknown never retried automatically |
| T5.9 | Frontend: after order creation call payment start and redirect; `/checkout/result/:number` polling page with paid / pending / failed (retry card or wallet on same order, CHK-09) / cancelled states; success view (CHK-10) with delivery date; analytics `purchase` once | E2E with fake provider: paid flow; failed then retried flow; closing tab after paying still confirms (webhook) |
| T5.10 | Emails via Outbox: `OrderPaid`, `LatePaymentRefunded`, `PaymentRefunded`, admin alerts (mismatch, unmatched transaction, unknown refund) in ar/en | Outbox dedupe test; templates render RTL correctly (snapshot) |
| T5.11 | **Manual sandbox check with real Paymob test keys** through a tunnel: card success, card failure, wallet success, refund; record results + any field differences in `docs/adr/` | Checklist in PR with screenshots/log excerpts (no secrets) |

**Exit criteria:** all payment integration tests green; sandbox check done; confirm inquiry path noted.

---

## M6 — Order management, shipping, returns, account & notifications

**Goal:** the owner runs daily operations from the admin panel; customers can follow and manage their orders.
Requirements: ADM-04, ADM-08, ORD-05, ORD-06, ORD-07, SHP-03, SHP-04, RET-01, RET-02, RET-03, RET-05, ACC-05, ACC-06, PAY-11, NTF-01, NTF-02, ACC-R1, CAT-09/RET-01 content.

| Task | What to do | Acceptance |
| --- | --- | --- |
| T6.1 | `OrderService` transition map + side effects (technical design §3.7); `POST /admin/orders/{id}/transition` offering only allowed targets; `RequiresReview` blocks Processing; clear-review endpoint | Unit test of the full allowed/forbidden matrix; integration tests for each side effect |
| T6.2 | Admin orders UI: status tabs with counts, search (number/phone/email), review filter, order detail (items, payment attempts & transactions, refunds, events timeline, notes with attachments), transition buttons, ship dialog (courier, tracking number, URL — SHP-04), refund dialog (amount, note, idempotency key), edit address/size before shipping (ORD-06, logged) | E2E: admin confirms → processing → shipped with tracking → delivered |
| T6.3 | Customer cancel (ORD-05) for `PendingPayment`/`Confirmed` → cancellation + automatic full refund when paid; guest tracking page `/track` (ORD-07, rate-limited, no address shown) | Tests: cancel paid order queues refund; tracking with wrong phone → not found |
| T6.4 | Returns (RET-02, RET-03, RET-05): admin records return (reason, photos) → `ReturnRequested`; refund from dialog; restock action after inspection creates StockMove; close → `ReturnClosed`; 14-day window check; alert when open > 5 days | Integration tests for window and restock |
| T6.5 | Account pages: my orders list/detail (status timeline, tracking link, invoice, cancel when allowed), addresses CRUD, settings (language), delete account (ACC-05, ACC-06) | E2E signed-in user sees order and cancels a Confirmed order |
| T6.6 | Invoice PDF (PAY-11) with QuestPDF: Arabic (RTL, Noto Sans Arabic embedded) always + English when order locale is en; order data snapshot only | Snapshot test of generated PDF text |
| T6.7 | Remaining emails (`OrderShipped`, `OrderCancelled`) and admin alert jobs (`ShippingAlertJob` SHP-03, `LowStockJob`); verify SPF/DKIM/DMARC instructions in README (NTF-02) | Outbox tests; alert job tests with fake time |
| T6.8 | Daily report (ADM-08): orders count, revenue (paid − refunded), refunds, orders by status, for a chosen date (Cairo time) | Test with fixed data |
| T6.9 | Policy pages content & footer links, returns summary on product page (RET-01), trust line wording (CAT-09) | Pages render in both languages |

**Exit criteria:** complete order lifecycle (create → pay → ship → deliver → return → refund) passes E2E in both languages.

---

## M7 — Hardening, release & go-live

**Goal:** meet every non-functional requirement, deploy to staging and production. Requirements: all NFR rows, SEO, analytics, D13.

| Task | What to do | Acceptance |
| --- | --- | --- |
| T7.1 | SEO: sitemap (both languages, published only), robots.txt, canonical, hreflang, JSON-LD Product + BreadcrumbList, Open Graph images | Rich Results Test passes on a product URL (staging) |
| T7.2 | Performance: image `srcset`, LCP image priority, route-level code splitting check, cache headers, Lighthouse CI budgets in `frontend.yml` | NFR web targets met on home/category/product (mobile) |
| T7.3 | Accessibility pass: axe in CI (zero serious/critical), manual keyboard + TalkBack checklist in `docs/qa/a11y.md` | Checklist complete |
| T7.4 | Security: CSP with nonces, headers review, OWASP ZAP baseline against staging, dependency audit, verify logs contain no PII/tokens, rate limits reviewed | ZAP: no high findings; report in `docs/qa/` |
| T7.5 | E2E suite complete (technical design §10.1) on mobile + desktop, ar + en; k6 load script in `tests/load/` | E2E green in CI; k6 run documented |
| T7.6 | Analytics (optional, env-toggled): GA4 + Meta Pixel events, `purchase` once per order | DebugView screenshot |
| T7.7 | Hosting decision (owner, D13) → ADR; production compose/infra files; HTTPS; managed or scheduled SQL backups (daily, 30 days); `deploy.yml` (staging auto, production manual approval, migrations step, rollback by image tag) | Staging live with test Paymob keys; restore test done once |
| T7.8 | Observability: Sentry both apps, uptime monitor on `/health`, alert email | Test alert received |
| T7.9 | UAT with 5 real people on staging (design system §8 usability: "buy a size L T-shirt" < 2 min), fix findings | Findings list closed or accepted by owner |
| T7.10 | Go-live checklist (section 9); switch to live Paymob keys, a domain mailbox (SPF/DKIM/DMARC) instead of Gmail, rotated Google client secret, and real content — all as production environment variables | All boxes ticked |

---

## 8. Requirement → task traceability (MVP)

| Requirement | Task(s) |
| --- | --- |
| ACC-R1 | T1.7, T3.3, T3.4, T4.2, T6.2 |
| ACC-R2 | T1.7 |
| ACC-R3 | T1.5, T1.7 |
| CAT-01 | T3.1, T3.5, T3.6 |
| CAT-02 | T3.1, T3.3 |
| CAT-03 | T3.3, T3.7 |
| CAT-04 | T3.6, T3.7 |
| CAT-05 | T3.5, T3.6 |
| CAT-06 | T3.6 |
| CAT-07 | T3.6 |
| CAT-08 | T3.5, T3.6 |
| CAT-09 | T3.6, T6.9 |
| CAT-10 | T3.2, T3.6 |
| CAT-11 | T3.5, T3.6 |
| CAT-12 | T3.6 |
| CART-01 | T4.1 |
| CART-02 | T4.1 |
| CART-03 | T4.6 |
| CART-04 | T4.6 |
| CART-05 | T4.3, T4.6 |
| CHK-01 | T4.7 |
| CHK-02 | T4.5, T4.7 |
| CHK-03 | T4.5, T4.7 |
| CHK-04 | T4.3 |
| CHK-05 | T4.3, T4.7 |
| CHK-07 | T4.5, T4.7 |
| CHK-08 | T4.4, T4.5 |
| CHK-09 | T5.4, T5.9 |
| CHK-10 | T5.9 |
| ACC-01 | T1.6, T2.5 |
| ACC-02 | T1.5, T2.5 |
| ACC-03 | T1.6 |
| ACC-04 | T4.5 |
| ACC-05 | T6.5 |
| ACC-06 | T1.8, T6.5 |
| PAY-01 | T5.2, T5.4 |
| PAY-02 | T5.3, T5.5 |
| PAY-03 | T5.5 |
| PAY-04 | T5.5 |
| PAY-05 | T5.5 |
| PAY-06 | T5.6, T5.7 |
| PAY-07 | T5.4, T5.7 |
| PAY-08 | T5.5 |
| PAY-09 | T5.8, T6.2 |
| PAY-10 | T5.2, T5.9 |
| PAY-11 | T6.6 |
| ORD-05 | T6.3 |
| ORD-06 | T6.2 |
| ORD-07 | T6.3 |
| SHP-01 | T4.2 |
| SHP-02 | T4.3 |
| SHP-03 | T6.7 |
| SHP-04 | T6.2 |
| RET-01 | T6.9 |
| RET-02 | T6.4 |
| RET-03 | T6.4 |
| RET-05 | T6.4 |
| NTF-01 | T1.5, T5.10, T6.7 |
| NTF-02 | T1.5, T6.7 |
| ADM-01 | T3.1b, T3.3, T3.7 |
| ADM-02 | T3.3, T3.7 |
| ADM-03 | T3.4 |
| ADM-04 | T6.1, T6.2 |
| ADM-06 | T4.2 |
| ADM-07 | T3.7, T6.9 |
| ADM-08 | T6.8 |
| ADM-09 | T4.8 |

---

## 9. Go-live checklist

**Payments & orders**

- [ ] Card, wallet, on mobile and desktop, in both languages — on production with small real amounts, then refunded
- [ ] Failed payment → retry on the same order with the other method
- [ ] Webhook blocked on purpose → order confirmed by reconciliation within 15 min
- [ ] Duplicate webhook → single confirmation
- [ ] 20 simultaneous orders on the last unit → exactly one succeeds
- [ ] Unpaid order cancelled after 15 min and stock released
- [ ] Cancel paid order → automatic full refund; partial refund from admin
- [ ] Full return: received → inspected → restocked → refunded
- [ ] Live Paymob keys set, inquiry path confirmed, wallet integration enabled (or consciously left off), no `Unknown` refunds open
- [ ] Gmail app password and Google client secret rotated; production secrets only in environment variables

**Operations & security**

- [ ] Backup restored on a separate environment
- [ ] Uptime & Sentry alerts received
- [ ] Rollback to the previous image works
- [ ] OWASP ZAP: no high findings; no secret in git; admin has 2FA
- [ ] SPF, DKIM, DMARC pass; emails land in Gmail & Outlook inbox

**Content**

- [ ] Terms, Privacy, Shipping, Returns, About, FAQ in both languages
- [ ] Shipping rules set for every governorate you deliver to
- [ ] All published products complete in both languages with photos
- [ ] Logo & favicon in place (or text wordmark approved)
- [ ] Search Console verified and sitemap submitted; analytics `purchase` fires once per order

---

## 10. After launch (operate & maintain)

**Weekly KPIs:** conversion rate (orders ÷ sessions), cart abandonment (> 70 % → review shipping & checkout), payment failure rate (sudden rise → integration or Paymob issue), returns per product (outlier → size guide or photos), delivery time vs promised per governorate, average order value vs free-shipping threshold, repeat customers, mobile LCP (> 2.5 s → images/JS).

**P2 backlog (in priority order):** coupons (CHK-11), wishlist (CAT-15), reviews (CAT-14), staff role (ADM-11), self-service returns & exchange (RET-06, RET-04), buy again & saved defaults (ACC-07, CHK-12), Meilisearch (CAT-13), full reports (ADM-12), Excel import/export (ADM-10), bulk actions (ADM-13), customers list (ADM-14), dark-mode toggle.
**P3:** shipping-company API (SHP-05), WhatsApp automation (NTF-03), marketing emails (NTF-04), Fawry & instalments, back-in-stock alerts (CAT-17). Cash on delivery only if the owner decides to add it later (D7).
