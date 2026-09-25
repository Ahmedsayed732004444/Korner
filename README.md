# Korner

Korner is an Egyptian online store (T-shirts, shoes, perfumes). Arabic (default, RTL) and English.
Online payment only, via Paymob (card + wallet).

Monorepo: `backend/` (ASP.NET Core 9 + SQL Server) and `frontend/` (React Router 7, framework mode, SSR).

## Docs

| File | Use it for |
| --- | --- |
| `docs/01-PRODUCT_SPEC.md` | Requirements by ID (`CHK-04`, `PAY-02`, …) |
| `docs/02-TECHNICAL_DESIGN.md` | Architecture, data model, API contract, Paymob integration, testing, CI/CD |
| `docs/03-DESIGN_SYSTEM.md` | Theme tokens, typography, layout, components, page specs |
| `docs/04-IMPLEMENTATION_PLAN.md` | Build order, task by task, Definition of Done |
| `docs/PROGRESS.md` | Which task is next, open questions, owner actions |
| `docs/SECRETS.md` | Where secrets live, status of each integration |

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) with [pnpm](https://pnpm.io/) (`corepack enable && corepack prepare pnpm@latest --activate`)
- [Docker](https://www.docker.com/) (SQL Server, Testcontainers for integration tests)
- Local secrets (git-ignored, not in this repo): `backend/src/Korner.Api/appsettings.Local.json` and `.env` — see `docs/SECRETS.md`

## Run the whole stack

```bash
docker compose up -d
```

This starts SQL Server, the API (`https://api.localhost` in dev, see `docker-compose.yml`) and the web app.
The web app calls the API's `/health` endpoint on its placeholder page once both are up.

## Backend

```bash
cd backend
dotnet build                      # must have 0 warnings
dotnet test                       # unit + integration (needs Docker for Testcontainers)
dotnet ef migrations add <Milestone>_<Change> -p src/Korner.Api -o Persistence/Migrations
dotnet run --project src/Korner.Api
```

## Frontend

```bash
cd frontend
pnpm install
pnpm dev
pnpm lint && pnpm typecheck && pnpm test
pnpm test:e2e                     # Playwright (needs the stack running with Payments:Provider=Fake)
pnpm gen:api                      # regenerate API types from the backend OpenAPI document
```

## Secrets

The repository is **public** — no secret is ever committed. Local values live in git-ignored files
(`backend/src/Korner.Api/appsettings.Local.json`, `.env`); production values are environment variables.
Before any commit, verify both are ignored:

```bash
git check-ignore -v backend/src/Korner.Api/appsettings.Local.json .env
```

See `docs/SECRETS.md` for what each key is for and its current status.

## Contributing

Read `CLAUDE.md` first — it has the non-negotiable rules this codebase follows (money handling, i18n/RTL,
accessibility, testing, secrets). Work follows `docs/04-IMPLEMENTATION_PLAN.md`, one task at a time; branch per
task group (`feat/m{n}-{short-name}`), Conventional Commits, PR via `gh pr create`.
