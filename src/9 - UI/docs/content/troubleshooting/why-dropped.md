---
title: Why was it dropped?
description: Common drop reasons and what to change.
icon: lucide:ban
---

**Dropped** means the message was rejected and not delivered to your inbox.

## Common reasons

### Wrong address (invalid recipient)
If the routing key or alias prefix is wrong, Nullbox can’t match it to your mailbox/alias.

Fix:

- double-check the full address you gave out
- copy/paste from the Nullbox UI when possible

### Auto-create is off
If you sent to an alias that doesn’t exist yet and auto-create is disabled for the mailbox, the message is dropped.

Fix:

- turn on **Automatically create new aliases**, or
- create the alias before using it

### Alias is disabled
Disabled aliases drop all inbound mail.

Fix:

- re-enable the alias, or
- migrate the service to a new alias

### Usage limits / policy
If forwarding the message would exceed a plan limit (for example, monthly bandwidth) or a policy rule decided it shouldn’t be delivered, it may be dropped.

Fix:

- reduce usage / upgrade plan (if applicable)
- adjust the alias settings depending on your intended workflow

## Where to see the reason

Your mailbox/alias activity feed shows the outcome for recent messages (Forwarded/Quarantined/Dropped) and often includes a reason tooltip for quarantined/dropped outcomes.

