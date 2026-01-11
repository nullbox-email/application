---
title: Data and privacy
description: What’s stored, what isn’t, and why.
icon: lucide:lock
---

Nullbox is an email relay. To forward (or quarantine) mail, Nullbox must receive messages addressed to your aliases and make a delivery decision.

This page explains what that means in practice.

## What Nullbox records for all inbound messages

To provide an audit trail and help you troubleshoot delivery, Nullbox records delivery metadata such as:

- sender and recipient details
- timestamps
- message size and attachment indicators
- subject line (and a subject hash)
- the final decision (forwarded, quarantined, or dropped) and the reason when available
- forwarding/provider delivery status (when applicable)

## What Nullbox does not keep for forwarded/dropped mail

For messages that are **forwarded** or **dropped**, Nullbox aims to avoid retaining full email body content beyond what is required to process and relay the message.

## Quarantine content

Quarantine is the one case where message content may need to be retained temporarily so you can review it and decide what to do next.

## Where your data goes

Your data stays within Nullbox infrastructure except for normal relay operations, where forwarded messages are delivered onward to your designated inbox provider.

## Retention (high level)

Nullbox keeps data only as long as needed to operate the service (delivery, audit/history, abuse prevention, and security). Quarantine content is kept only as long as needed to support the quarantine workflow.

