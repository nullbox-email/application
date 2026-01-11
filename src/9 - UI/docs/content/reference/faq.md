---
title: FAQ
description: Common questions and practical answers.
icon: lucide:help-circle
---

## What’s the difference between a mailbox and an alias?

- A **mailbox** is the destination (your real inbox) plus your private domain like `<routing-key>.nullbox.email`.
- An **alias** is a specific address under that domain (like `amazon@<routing-key>.nullbox.email`).

## What is a routing key?

The **routing key** is the unique part of your mailbox domain. It separates your mailbox from everyone else’s and lets Nullbox route inbound mail to you.

## Can I change an alias email address after creating it?

You can rename the alias (the label you see in the UI), but the **email prefix** (the part before `@`) cannot be changed after the alias is created.

If you need a new address, create a new alias and migrate the service.

## What happens if I disable an alias?

Disabled aliases **drop** inbound mail. This is the fastest way to stop spam, but it also blocks legitimate mail to that address.

## What is Learning mode?

Learning mode is “default allow” for new senders on an alias:

- unknown senders are forwarded, and
- Nullbox learns them as trusted for that alias

New aliases start in Learning mode for about **30 days** by default.

## What is Active protection?

Active protection is “default quarantine” for unknown senders on an alias. It’s a good setting for sensitive addresses (logins, financial services, important receipts).

## What is Direct passthrough?

Direct passthrough forwards mail without applying quarantine and safety checks for unknown senders. It’s useful temporarily (for example, during vendor verification), but it increases spam/phishing risk.

## Why was an email quarantined?

Common reasons include:

- the sender is unknown and the alias is in Active protection
- the sender looks suspicious
- a policy rule decided it should not be delivered directly

See [Quarantine](/concepts/quarantine) and [Sender protection](/concepts/sender-protection).

## Does Nullbox store my email content?

Nullbox keeps an audit trail (delivery metadata and decision outcomes) so you can see what happened to inbound messages.

Quarantined messages may require temporarily retaining message content so you can review them. See [Data and privacy](/reference/data-and-privacy) for details.

## Can I use an alias as my “mailbox address” (like replacing Gmail)?

Usually no. Your real inbox is the destination you forward to; changing it to an alias can create loops and break account recovery.

See [What not to put behind an alias](/guides/what-not-to-alias).

