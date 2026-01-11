---
title: Protect important accounts
description: Use stricter settings for logins, financial services, and critical receipts.
icon: lucide:shield
---

Some accounts are too important to treat like a newsletter signup.

This guide helps you use aliases for high-value accounts while reducing the risk of missing critical email.

## Recommended setup

::steps
### Create a dedicated alias per service
Examples:

- `bankname@<your-routing-key>.nullbox.email`
- `registrar@<your-routing-key>.nullbox.email`
- `password-manager@<your-routing-key>.nullbox.email`

### Enable quarantine and safety checks
This keeps sender protection active (so you can quarantine unknown or suspicious mail).

### Switch to Active protection after the “learning period”
Once you’ve received a few legitimate messages from the service, enable **Active protection** so unknown senders are quarantined by default.

### Temporarily loosen settings when you make changes
If you’re changing login email, resetting a password, or expecting a one-time code:

- temporarily enable **Learning mode** (or use **Direct passthrough** briefly)
- complete the verification
- switch back to **Active protection**
::

## Don’t break recovery flows

Before you move a critical account onto an alias, make sure you have strong recovery options enabled (2FA, backup codes, recovery email/phone).

See [What not to put behind an alias](/guides/what-not-to-alias) for the safe default rule.

