# CLAUDE.md — Korner

Korner is an Egyptian online store (T-shirts, shoes, perfumes). Arabic (default, RTL) + English.
Online payment only via Paymob (card + wallet). Monorepo: `backend/` (ASP.NET Core 9 + SQL Server) and
`frontend/` (React Router 7 framework mode, SSR).

## Read first

| File | Use it for |
| --- | --- |
| `docs/04-IMPLEMENTATION_PLAN.md` | What to build next, task by task, and the Definition of Done |
| `docs/PROGRESS.md` | Which task is next (first unticked), open questions |
| `docs/01-PRODUCT_SPEC.md` | Requirements by ID (`CHK-04`, `PAY-02` …) — the source of truth for behaviour |
| `docs/02-TECHNICAL_DESIGN.md` | Architecture, data model, API contract, Paymob integration, testing, CI/CD |
| `docs/03-DESIGN_SYSTEM.md` | Theme tokens (`app.css`), typography, layout, components, states, page specs |
| `docs/SECRETS.md` | Where secrets live (git-ignored files), status of each integration, what to report to the owner |

Work on **one task at a time** in plan order. Before coding, read only the spec sections the task references.
If a spec is ambiguous or wrong, stop and ask (write it in the PR and in `docs/PROGRESS.md` → Questions). Do not invent requirements.

## Non-negotiable rules

1. **Money is `long` piasters** everywhere (SQL `bigint`). No `decimal`/`float`/`double` for money. The frontend never calculates prices or totals — it displays what the API returns via `formatMoney`.
2. **Server is the source of truth** for prices, stock, totals, order status and payment status. A redirect from Paymob never confirms an order; only a verified webhook or an inquiry does.
3. **Every stock or money change** is a guarded SQL update (compare-and-set) inside a DB transaction. Never read-modify-write stock or balances in C#.
4. **Order status changes only through `OrderService.TryTransitionAsync`** and only along the state machine in product spec §8.1.
5. **Services return `Result`/`Result<T>`**; controllers only map to `Ok(...)` / `ToProblem()`. Errors carry stable codes (`Inventory.OutOfStock`), translated in the frontend (`locales/{ar,en}/errors.json`).
6. **No secrets in git — the repo is public.** Local values are in `backend/src/Korner.Api/appsettings.Local.json` and `.env` (both git-ignored; run `git check-ignore` before committing). Never print, log or paste a secret. Details and the status of every key: `docs/SECRETS.md`.
7. **No tokens in `localStorage`/`sessionStorage`.** Access token in memory; refresh token in an httpOnly cookie set by the API.
8. **i18n & RTL:** no hard-coded UI strings; every key exists in `ar` and `en`. Use logical Tailwind utilities only (`ms-/me-/ps-/pe-/start-/end-/text-start`), never `ml-/mr-/pl-/pr-/left-/right-`.
9. **Design tokens only** — no hex colours or off-scale spacing in components. `--brand` is decoration only (never text).
10. **Accessibility:** WCAG 2.2 AA, 44 px touch targets, visible focus ring, labels above inputs, errors linked with `aria-describedby`.
11. **No `console.*`** in frontend code (use `lib/logger.ts`); backend logs never contain tokens, phone numbers, emails or full addresses.
12. **Tests are part of the task.** Anything touching money, stock, auth or payments needs integration tests (Testcontainers SQL Server).
13. **Permission flags in responses:** return server-computed `isMine`, `canEdit`, `canDelete`, `canCancel`, `canRefund`, `canPublish` + `publishBlockers`, `allowedTransitions`, … from the same policy that enforces the action. The UI shows actions only from these flags.
14. **Disabled integrations are reported, not hidden:** if a key/ID is missing or `0` (e.g. Paymob wallet `MobileIntegrationId = 0`) or an external service rejects the credentials, disable that feature cleanly and tell the owner in the PR and in `docs/PROGRESS.md` → *Owner actions*.
15. **Images are stored on the API server** (`IFileStorage` → `wwwroot/uploads`, WebP 480/960/1440), never a cloud image service. Store relative paths only.
16. **Owner's recipes:** for Hangfire, HybridCache and rate limiting follow `https://github.com/Ahmedsayed732004444/dotnet-recipes` (clone it outside this repo for reference).
17. Only build `MVP` requirements. `P2`/`P3` items are out of scope unless a task says otherwise. Cash on delivery and shipping-company APIs are **out of scope**.

## Commands

```bash
# whole stack (SQL Server + api + web)
docker compose up -d

# backend
cd backend
dotnet build                      # must have 0 warnings
dotnet test                       # unit + integration (needs Docker for Testcontainers)
dotnet ef migrations add <Milestone>_<Change> -p src/Korner.Api -o Persistence/Migrations
dotnet run --project src/Korner.Api

# frontend
cd frontend
pnpm install
pnpm dev
pnpm lint && pnpm typecheck && pnpm test
pnpm test:e2e                     # Playwright (needs the stack running with Payments:Provider=Fake)
pnpm gen:api                      # regenerate API types from the backend OpenAPI document
```

## Git workflow

- Branch per task group: `feat/m{n}-{short-name}`; never push to `main` directly.
- Conventional Commits with task and requirement IDs: `feat(payments): verify Paymob HMAC [T5.3, PAY-02]`.
- Open a PR with `gh pr create`: summary, requirement IDs, tests run, screenshots (360 px + 1280 px, ar + en) for UI.
- Tick the task in `docs/PROGRESS.md` in the same PR.

## Project facts

- Order number format `K-YYMMDD-NNNN`. Payment attempt reference `{orderNumber}-P{n}` (Paymob `special_reference`).
- Storefront `https://{DOMAIN}`, API `https://api.{DOMAIN}`; Arabic at `/`, English at `/en/`.
- Governorates: 27 codes in `docs/data/governorates.json` (single source for backend and frontend).
- Images: `IFileStorage`/`LocalFileStorage`; DB stores `uploads/products/{guid}`; URLs from `Storage:PublicBaseUrl`.
- Background jobs: Hangfire (`KornerJobs` DB, dashboard `/jobs`). Catalogue caching: `HybridCache`.
- Payments in tests/E2E use `FakePaymentProvider` (`Payments:Provider=Fake`); real Paymob sandbox only in the manual check of task T5.11.
