---
title: Mailboxes and routing keys
description: Your private Nullbox domain and where forwarded mail ends up.
icon: lucide:inbox
---

In Nullbox, a **mailbox** is the “container” that receives inbound email and forwards it to your real inbox.

Every mailbox has:

- a **forwarding address** (your real inbox destination)
- a private domain in the form **`<routing-key>.nullbox.email`**
- settings that control whether new aliases can be created automatically

## What is a routing key?

A **routing key** is the unique part of your mailbox domain.

If your routing key is `ab12cd`, your mailbox domain is:

- `ab12cd.nullbox.email`

Aliases under that mailbox look like:

- `amazon@ab12cd.nullbox.email`
- `netflix@ab12cd.nullbox.email`

Think of the routing key as your “alias namespace.” You don’t need to memorize it—just copy/paste it when you need it.

## Auto-create aliases (mailbox-level)

When **Automatically create new aliases** is enabled for a mailbox:

- Mail sent to a brand-new prefix (like `newvendor@ab12cd.nullbox.email`) can create that alias automatically.
- This makes it easy to invent new addresses without pre-registering them.

When it’s disabled:

- Only aliases you already created will receive mail.
- Mail to a missing alias is dropped.

## Multiple mailboxes

You can create multiple mailboxes if you want separate “domains” and destinations (for example: personal vs work).

Each mailbox has its own routing key, its own aliases, and its own settings.

## Limits (high level)

Depending on your plan, there can be limits such as:

- number of mailboxes per account
- number of aliases per mailbox
- monthly forwarding bandwidth

When a limit would be exceeded, Nullbox may drop the message instead of forwarding it.

## Advanced: what forwarded mail may look like

Forwarded messages may show a Nullbox-generated “from” address to support reliable delivery and reply handling.

You can always see what happened to an inbound message in the audit/activity feed (forwarded, quarantined, or dropped) along with a reason when available.

