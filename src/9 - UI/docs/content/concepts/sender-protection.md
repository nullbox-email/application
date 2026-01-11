---
title: Sender protection
description: How Nullbox learns trusted senders and handles unknown or suspicious mail.
icon: lucide:shield-check
---

Sender protection answers one question:

“For this alias, should mail from this sender be forwarded, quarantined, or dropped?”

Nullbox evaluates senders **per alias**, so you can have strict rules for a login alias and looser rules for a newsletter alias.

## The two controls that matter most

### Quarantine and safety checks vs Direct passthrough

- **Quarantine and safety checks:** sender decisions are applied (forward/quarantine/drop).
- **Direct passthrough:** mail is forwarded without quarantine checks for unknown senders.

### Learning mode vs Active protection

- **Learning mode:** unknown senders are forwarded and learned as trusted.
- **Active protection:** unknown senders are quarantined by default.

New aliases start in **Learning mode** for about **30 days** by default, then switch to **Active protection** automatically.

## What “learning” means (plain language)

In Learning mode, the first time a real sender emails an alias:

- the message is forwarded, and
- that sender becomes “known” for that alias going forward

This keeps migrations smooth (you don’t miss first-time verification emails) while still building a trusted sender set over time.

## Suspicious senders

In Active protection, Nullbox may quarantine some “weird-looking” senders more aggressively (for example, unusual domain formatting). This is a safety feature—not a statement that the sender is definitely malicious.

## Advanced: simplified sender decision logic

This is an approximation of the behavior for a forwarded-eligible message:

```
if direct_passthrough:
  forward
else if explicit_rule_exists:
  follow rule (forward / quarantine / drop)
else if learning_mode:
  forward and learn sender
else:
  quarantine (unknown/suspicious sender)
```

After a forward decision is made, account limits can still prevent forwarding (see [Inbound email](/how-it-works/process-email)).

