# Secrets & external services

> This file contains **no secret values**. Values live in git-ignored files only:
> `backend/src/Korner.Api/appsettings.Local.json` (API) and `.env` (Docker). The repository is **public** —
> a committed secret is compromised the moment it is pushed.

## Rules for Claude Code

1. Before the first commit of any session run `git check-ignore -v backend/src/Korner.Api/appsettings.Local.json .env`; both must be ignored. If not, fix `.gitignore` before anything else.
2. Never print, log, echo, test-assert or paste a secret value (in code, tests, PRs, commit messages or chat). Refer to keys by name only.
3. Load `appsettings.Local.json` only in Development: `if (builder.Environment.IsDevelopment()) builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);`
4. Tests and CI never use these values: integration tests use Testcontainers + `FakePaymentProvider` + a fake email sender; CI uses no secrets.
5. When an integration does not work with the provided values, **stop and tell the owner** (PR + `docs/PROGRESS.md` → *Owner actions*) with the exact error code/message — never work around it silently.

## Status of the provided values (verify during the listed task)

| Integration | Section | Status now | Verify in | What to report to the owner if it fails |
| --- | --- | --- | --- | --- |
| SQL Server (Docker) | `ConnectionStrings` | Generated for local dev | T0.4 | — |
| JWT signing key | `Jwt:Key` | Generated (64 random bytes) | T1.5 | — |
| Google sign-in | `Authentication:Google` | Provided | T1.6 | `redirect_uri_mismatch` → add `https://localhost:5081/signin-google` (and later `https://api.<domain>/signin-google`) to *Authorised redirect URIs* in Google Cloud Console; consent screen must be *In production* or the test user added |
| Email (Gmail SMTP) | `MailSettings` | Provided (Gmail app password) | T1.5 | `535 authentication failed` → app password revoked or 2-Step Verification off. Gmail limit ≈ 500 emails/day and no SPF/DKIM for your domain → move to a domain mailbox before launch (task T7.10) |
| Paymob card | `PaymobSettings:CardIntegrationId` + keys | Provided (**test** mode) | T5.11 | `404 Integration ID/Name does not exist` → ID and secret key are from different modes |
| Paymob wallets | `PaymobSettings:MobileIntegrationId` | **`0` = not activated** | T5.2 | Wallet payments stay hidden until the owner creates a *Mobile Wallet* integration in the Paymob dashboard and sends its ID |
| Paymob webhook | `PaymobSettings:ApiPublicUrl` | Placeholder `https://CHANGE-ME.trycloudflare.com` | T5.11 | Owner/Claude Code runs `cloudflared tunnel --url https://localhost:5081` and puts the URL here |
| Paymob inquiry path | `PaymobSettings:InquiryPath` | Default value, unconfirmed | T5.11 | Owner confirms the path in *Developers → API explorer* |
| Paymob payouts | `PayoutSettings` | Provided but **out of scope** | — | Not used by Korner |
| Hangfire dashboard | `HangfireSettings` | Generated | T1.3 / T5.7 | — |
| Seed admin | `Seed` | Admin email = owner's Gmail, generated password | T1.3 | Owner changes the password after first login and enables 2FA |
| Live Paymob keys, domain mailbox, Sentry DSN | — | Not provided yet | T7.7–T7.10 | Needed before production |

## Recommended by Claude (owner decision)

The Gmail app password and the Google client secret were shared in a chat. They are fine for local development,
but **rotate both before going to production** (new app password in the Google account security page; new client
secret in Google Cloud Console) and put the new values only in the production environment variables.
