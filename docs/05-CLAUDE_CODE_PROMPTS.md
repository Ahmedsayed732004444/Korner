# Prompts to give Claude Code

Copy one prompt per session. Each milestone may take several sessions; the next session just repeats
"Continue" (prompt 2) until the milestone's exit criteria are met, then use the review prompt.

## 1. First session (repository bootstrap)

```
You are building "Korner", an Egyptian online store, in this repo:
https://github.com/Ahmedsayed732004444/Korner.git (monorepo: backend/ + frontend/ + docs/).

1. The files CLAUDE.md and docs/ (01-PRODUCT_SPEC.md, 02-TECHNICAL_DESIGN.md, 03-DESIGN_SYSTEM.md,
   04-IMPLEMENTATION_PLAN.md, 05-CLAUDE_CODE_PROMPTS.md, SECRETS.md, data/governorates.json) are already in the repo,
   plus the git-ignored local secrets (backend/src/Korner.Api/appsettings.Local.json and .env).
   First run: git check-ignore -v backend/src/Korner.Api/appsettings.Local.json .env  (both MUST be ignored).
   Read CLAUDE.md and docs/04-IMPLEMENTATION_PLAN.md fully.
2. Execute milestone M0 task by task (T0.1 → T0.5) following the session protocol in section 2 of the plan.
3. After each task: run the checks, update docs/PROGRESS.md, commit with a Conventional Commit, push,
   and open a PR with `gh pr create`.
Do not start M1. If anything in the specs is unclear, stop and ask me.
```

## 2. Continue (any later session)

```
Read CLAUDE.md and docs/PROGRESS.md. Take the first unticked task of the current milestone in
docs/04-IMPLEMENTATION_PLAN.md, read only the spec sections it references, write a short plan,
implement it with its tests, make all checks green, update PROGRESS.md, commit, push and open a PR.
Stop after one task and summarise what you did, what you tested and any open question.
```

## 3. Milestone review (before moving on)

```
We finished milestone M{n}. Review the whole milestone against docs/04-IMPLEMENTATION_PLAN.md:
for every task and every requirement ID listed for M{n}, show where it is implemented and which test proves it.
Run all backend and frontend checks. List anything missing or weak as new unticked tasks in PROGRESS.md
(prefix "Fix:"). Do not start the next milestone.
```

## 4. Payments sandbox check (task T5.11)

```
Do task T5.11. The Paymob test values are already in backend/src/Korner.Api/appsettings.Local.json
(PaymobSettings). Start a tunnel (cloudflared tunnel --url https://localhost:5081), put its URL in
PaymobSettings:ApiPublicUrl, set Payments:Provider = Paymob, then walk me through a card success, a card failure
and a refund, checking the database after each. Wallets stay disabled while MobileIntegrationId = 0 — tell me
if that is still the case. Record any payload difference from the design in docs/adr/. Never print the keys.
```
