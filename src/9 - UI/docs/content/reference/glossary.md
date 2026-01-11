---
title: Glossary
description: Definitions of key Nullbox terms and settings.
icon: lucide:book-open
---

## Mailbox

A container that forwards inbound email to your real inbox. Each mailbox has its own private domain (your routing key).

## Routing key

The unique identifier in your mailbox domain, like `ab12cd` in `ab12cd.nullbox.email`.

## Alias

An email address under your mailbox domain (example: `amazon@ab12cd.nullbox.email`). You give aliases to services instead of reusing your real inbox address.

## Email prefix (local part)

The part before `@` in an alias address (example: `amazon` in `amazon@ab12cd.nullbox.email`).

## Auto-create aliases

A mailbox setting that lets Nullbox create an alias automatically when mail arrives for a prefix that doesn’t exist yet.

## Enabled / Disabled alias

- **Enabled:** mail can be forwarded/quarantined (based on protection settings).
- **Disabled:** inbound mail is dropped.

## Quarantine and safety checks

Alias setting that enables sender decisions and safety checks (forward/quarantine/drop).

## Direct passthrough

Alias setting that bypasses quarantine and safety checks for unknown senders and forwards mail (if the alias is enabled).

## Learning mode

Alias setting where unknown senders are forwarded and learned as trusted for that alias.

## Active protection

Alias setting where unknown senders are quarantined by default.

## Forward / Quarantine / Drop

The three outcomes Nullbox can apply to an inbound message:

- **Forward:** delivered to your real inbox
- **Quarantine:** held back as unsafe/untrusted
- **Drop:** rejected and not delivered

## Activity feed / audit trail

A history of inbound messages and what happened to them (forwarded, quarantined, or dropped), often with a reason.

## Provider status

A forwarding status indicator (for example: pending/succeeded/failed) associated with delivery attempts.

