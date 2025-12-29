---
title: What not to put behind an alias
description: Your mailbox (like Gmail) is the destination. Use aliases for everything else.
icon: lucide:ban
---

Nullbox aliases are meant to be the addresses you give to *other services*.

Your real mailbox (for example, Gmail) is where the forwarded mail ultimately lands.

## Don’t use an alias for “your mailbox”

Examples of “your mailbox” services:

- Gmail / Google Workspace
- Outlook.com / Microsoft 365
- iCloud Mail
- Proton Mail
- Yahoo Mail

Why:

- These accounts are the endpoint you’re forwarding *to*
- Changing them to use an alias can break login/recovery flows
- It can create confusing loops (“forwarding to the thing I’m forwarding from”)

::alert{type="info" title="This is the safe rule" icon="lucide:info"}
Keep your mailbox email address as-is. Create aliases that forward into it.
::

## Use aliases for (almost) everything else

Good candidates:

- Shopping: Amazon, Walmart, etc.
- Streaming: Netflix, Hulu, Disney+
- Newsletters and marketing lists
- Forums, apps, SaaS tools
- Travel, food delivery, ride share

If you’re unsure, ask: “If this vendor leaked my email, would I want to turn it off instantly?” If yes, use an alias.

## High-risk accounts (alias only if you’re confident)

Some accounts are so important that you should be deliberate:

- Domain registrar / DNS provider
- Password manager
- Primary identity / SSO accounts

You *can* use an alias here, but only if you’ve verified you still have strong recovery options (2FA, backup codes, recovery email/phone).

