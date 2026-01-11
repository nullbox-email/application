---
title: An email didn’t arrive
description: A fast checklist for “where did it go?”
icon: lucide:mail-x
---

If an email didn’t show up where you expected, the goal is to answer one question quickly:

Did Nullbox **forward**, **quarantine**, or **drop** it?

## Quick checklist

::steps
### Confirm the address is correct
Make sure you sent to the full alias address:

- `alias@<your-routing-key>.nullbox.email`

Typos in the routing key or alias prefix will usually result in a drop.

### Check the activity/audit feed
Open the mailbox or alias dashboard and look at recent messages.

You should see an outcome like:

- Forwarded
- Quarantined (usually with a reason)
- Dropped (usually with a reason)

### If it was dropped
Common causes:

- the alias is disabled
- auto-create is off and the alias does not exist
- a usage/plan limit would be exceeded

See [Why was it dropped?](/troubleshooting/why-dropped).

### If it was quarantined
Common causes:

- Active protection is enabled and the sender is new/unknown
- the sender was considered suspicious

If you need the message immediately, temporarily enable **Learning mode** or **Direct passthrough** and ask the sender to resend.

### If it was forwarded but you still don’t see it
Check your inbox spam/junk folders and any server-side filters.
::

## Still stuck?

Read the full inbound pipeline in [Inbound email](/how-it-works/process-email), then compare it to the reason shown in your activity feed.

