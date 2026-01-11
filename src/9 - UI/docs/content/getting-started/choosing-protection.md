---
title: Choosing protection
description: Pick between Learning Mode, Active Protection, and Direct Passthrough.
icon: lucide:shield
---

Each alias has two independent controls:

1. **Quarantine and safety checks** vs **Direct passthrough**
2. **Learning mode** vs **Active protection**

The right choice depends on how risky the alias is and whether you can tolerate “missing” a first-time sender.

## Quick recommendations

- **New alias / migrations / verification emails:** Learning mode (or temporary passthrough), then tighten later.
- **Important logins (banking, registrar, password manager):** Active protection + quarantine and safety checks.
- **Low-risk signups (one-time coupons, trial accounts):** Learning mode is usually fine.

## Quarantine and safety checks vs Direct passthrough

### Quarantine and safety checks
Nullbox applies sender rules and safety checks and can **quarantine** or **drop** messages before they reach your inbox.

### Direct passthrough
Nullbox forwards messages (if the alias is enabled), **bypassing quarantine and most sender checks**.

::alert{type="warning" title="Use passthrough sparingly" icon="lucide:triangle-alert"}
Direct passthrough is great for short windows (like account verification), but it also increases spam and phishing risk. Turn it off when you’re done.
::

## Learning mode vs Active protection

### Learning mode
Learning mode is “default allow for new senders.”

- Unknown senders are **forwarded**
- The sender is **learned** as trusted for that alias (so future mail can be handled consistently)

### Active protection
Active protection is “default deny for new senders.”

- Unknown senders are **quarantined** by default
- Only already-trusted senders are forwarded automatically

New aliases start in **Learning mode** for about **30 days** by default, then switch to **Active protection** automatically. You can change this any time.

## What happens to a brand-new sender?

| Alias mode | Default for unknown senders |
| --- | --- |
| Learning mode | Forward (and learn) |
| Active protection | Quarantine |

## Advanced: simplified decision order

This is a simplified version of how an inbound message is evaluated for a given alias.

```
1) Validate the mailbox domain (your routing key)
2) Find or auto-create the alias (if enabled)
3) If alias is disabled: drop
4) If direct passthrough: forward
5) Else:
   - If an explicit sender rule exists: follow it
   - Else if learning mode: forward and learn the sender
   - Else: quarantine (unknown/suspicious sender)
6) If forwarding would exceed limits: drop
```

For the full inbound pipeline, see [Inbound email](/how-it-works/process-email).

