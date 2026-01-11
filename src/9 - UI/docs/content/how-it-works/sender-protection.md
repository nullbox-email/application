---
title: Sender protection internals
description: How sender rules, learning mode, and passthrough affect Forward/Quarantine/Drop.
icon: lucide:shield-check
---

Sender protection is evaluated **per alias** and answers:

“For this alias, what should we do with mail from this sender?”

This page explains the behavior in a way that’s friendly to non-technical readers, with a precise ruleset for technical readers at the end.

## The three moving pieces

### 1) Enabled vs disabled

- **Enabled alias:** mail can be forwarded/quarantined/dropped based on settings.
- **Disabled alias:** mail is dropped.

### 2) Quarantine and safety checks vs Direct passthrough

- **Quarantine and safety checks:** sender rules and safety checks are evaluated.
- **Direct passthrough:** mail is forwarded without quarantine checks for unknown senders.

### 3) Learning mode vs Active protection

- **Learning mode:** first-time senders are allowed through and become “known” for the alias.
- **Active protection:** first-time senders are quarantined by default.

New aliases start in Learning mode for about **30 days** by default.

## What happens for a first-time sender (most common question)

| Alias settings | What happens to an unknown sender? |
| --- | --- |
| Quarantine and safety checks + Learning mode | Forwarded (and learned) |
| Quarantine and safety checks + Active protection | Quarantined (unknown/suspicious) |
| Direct passthrough | Forwarded |

## Auto-created aliases (first delivery)

If your mailbox has auto-create enabled, the first email to a brand-new alias can create that alias automatically.

On that first delivery, Nullbox treats the alias like it’s in Learning mode:

- the first sender is allowed through, and
- a learned “allow” decision can be recorded for that sender

This is designed to reduce the chance of missing a vendor verification email on first use.

## Suspicious sender heuristics (high level)

When Active protection is enabled, Nullbox may quarantine some “weird-looking” sender domains more aggressively (for example, punycode or non-ASCII domains). This is a conservative safety measure.

## Technical ruleset (simplified)

```
if alias_disabled:
  drop

if direct_passthrough:
  forward
else if sender_rule_exists_and_enabled:
  follow rule (forward / quarantine / drop)
else if learning_mode (or alias was just auto-created):
  forward and learn sender
else:
  quarantine (unknown or suspicious sender)

if decision == forward and forwarding_would_exceed_limits:
  drop
```

