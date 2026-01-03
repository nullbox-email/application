# Nullbox

**Email aliases that protect your inbox without replacing it.**

Nullbox is an email aliasing and relay system designed to reduce inbox noise and limit email address exposure. It lets you create unique addresses per service, forward mail into your existing inbox, and shut off spam at the alias level without changing your real email.

Nullbox does not replace your inbox. It sits in front of it.

The entire system is source available and self hostable.

## What Nullbox is for

Use Nullbox whenever a site asks for your email address but you do not want to share your primary inbox.

Common examples:
- Shopping and online accounts
- Newsletters and mailing lists
- SaaS tools and trials
- Forums and community sites
- Travel, food delivery, and subscriptions

Each service gets its own address. If an address leaks or starts receiving spam, it can be disabled or rotated without affecting anything else.

## How it works

At a high level:

1. Incoming mail is received by a Cloudflare Email Worker
2. The worker checks alias and mailbox metadata via the Nullbox API
3. Messages are forwarded, quarantined, or dropped
4. Forwarded messages are sent directly to your existing inbox

Only minimal metadata is processed, and only when required. Message content is not stored long term, and Nullbox does not act as a mailbox provider.

## Key features

- **Private mailbox domains**  
  Each mailbox has its own domain used exclusively for aliases.

- **Alias per service**  
  Create, disable, and rotate aliases independently.

- **Forwarding to your real inbox**  
  Mail is forwarded to Gmail, Outlook, Proton, or any other provider you already use.

- **Unexpected sender quarantine**  
  Messages from new or suspicious senders can be held instead of forwarded.

- **Minimal data surface**  
  Designed to collect and retain as little data as possible.

## Repository overview

This repository contains the full Nullbox application, including:

- Aspire AppHost for service orchestration
- .NET APIs for core services
- Nuxt applications for the web UI and authenticated app
- Cloudflare Email Worker for email ingress
- Documentation and supporting tooling

Everything required to run Nullbox is included here.
