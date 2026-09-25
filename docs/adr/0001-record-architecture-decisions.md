# 1. Record architecture decisions

## Status

Accepted

## Context

Korner is built iteratively across several milestones (`docs/04-IMPLEMENTATION_PLAN.md`). Decisions made while
implementing a task — an accepted deviation from `02-TECHNICAL_DESIGN.md`, a field difference discovered in a
third-party API (e.g. Paymob), a hosting choice — need a durable record so later sessions do not re-litigate or
silently diverge from them.

## Decision

We use Architecture Decision Records (ADRs) as described by Michael Nygard, one Markdown file per decision in
`docs/adr/`, numbered sequentially (`NNNN-title.md`). Each ADR has: Status (`Proposed`/`Accepted`/`Superseded`),
Context, Decision, Consequences.

An ADR is added whenever:

- A task's implementation departs from `02-TECHNICAL_DESIGN.md` or `03-DESIGN_SYSTEM.md` in a way the owner accepted.
- A real integration (Paymob, Google, Gmail) behaves differently from the spec (e.g. a field name, an error shape).
- A cross-cutting choice is made that later tasks must follow (e.g. a hosting decision in T7.7).

Routine implementation detail that only affects one task does not need an ADR — use the PR description instead.

## Consequences

Future sessions read `docs/adr/` before assuming the technical design is exactly what is implemented. ADRs are
never deleted; a changed decision gets a new ADR that supersedes the old one (old one's Status updated to
`Superseded by NNNN`).
