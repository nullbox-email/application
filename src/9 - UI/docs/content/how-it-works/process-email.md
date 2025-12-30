---
title: Inbound email
description: How Nullbox decides to forward, quarantine, or drop an inbound email.
icon: lucide:mail
-----------------

When an email arrives at a Nullbox address, Nullbox decides what to do with it based on your settings and a few safety checks. There are only three possible outcomes:

* **Forward**: delivered to your real inbox
* **Quarantine**: held for review
* **Drop**: rejected and not delivered

## What Nullbox checks

### Is this address valid?

Nullbox first confirms the message was sent to a real Nullbox address you own.

* If the address doesn’t exist (or the domain isn’t yours), the message is **dropped**.

### Does the alias exist (or should it be created)?

If the domain is yours but the specific alias hasn’t been created yet, Nullbox may create it automatically *if you’ve enabled that*.

* If auto-create is enabled and your account limits allow it, the message can be **forwarded**.
* If not, the message is **dropped**.

### Is the alias active?

If the alias exists but has been disabled, the message is **dropped**.

### Is the sender allowed?

If the message is eligible to be forwarded, Nullbox applies your sender protection rules:

* If your alias is set to allow all senders, the message is **forwarded**.
* Otherwise:

  * Known/approved senders are **forwarded**
  * Unknown or suspicious senders are usually **quarantined** so you can review them

### Are you within usage limits?

Finally, Nullbox checks whether forwarding the message would exceed account limits (for example, monthly bandwidth).

* If limits would be exceeded, the message is **dropped**
* Otherwise, it proceeds as **forwarded**

## Privacy and message content

Nullbox does **not** capture or store email body content through the API for messages that are **forwarded** or **dropped**.

* For forwarded/dropped messages, Nullbox only processes and records **telemetry and delivery metadata** (for example: sender/recipient details, timestamps, sizes, and the final decision).
* If a message is **quarantined**, Nullbox stores the **email body** so you can review it and decide what to do.
* Once you decide, the stored quarantine content is only kept as long as needed to support that workflow.

## What you can expect as a user

* Most normal mail is **forwarded** automatically.
* Anything questionable can be **quarantined** so you can decide.
* Invalid recipients, disabled aliases, or messages blocked by limits are **dropped**.

## Audit trail

Nullbox records what happened for each inbound message so you can see outcomes in your activity/history views (forwarded, quarantined, or dropped) and understand why.
