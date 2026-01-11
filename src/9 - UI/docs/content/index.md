---
title: Home
navigation: false
---

::hero
---
announcement:
  title: 'Start here'
  icon: 'lucide:rocket'
  to: /getting-started/quickstart
actions:
  - name: Quickstart
    to: /getting-started/quickstart
  - name: How It Works
    variant: outline
    to: /how-it-works/process-email
  - name: Guides
    variant: outline
    to: /guides
---

#title
Nullbox Email Docs

#description
Set up mailboxes and aliases, understand delivery decisions, and keep spam out of your inbox.
::

::card-group{cols="2"}
  ::card{title="Quickstart" icon="lucide:rocket" to="/getting-started/quickstart" description="Create a mailbox and your first alias in a few minutes."}
  ::
  ::card{title="Choosing protection" icon="lucide:shield" to="/getting-started/choosing-protection" description="Learning mode vs active protection vs passthrough."}
  ::
  ::card{title="Inbound email processing" icon="lucide:mail" to="/how-it-works/process-email" description="How Nullbox decides Forward / Quarantine / Drop."}
  ::
  ::card{title="Sender protection internals" icon="lucide:shield-check" to="/how-it-works/sender-protection" description="How learning mode and sender rules work under the hood."}
  ::
  ::card{title="Stop spam fast" icon="lucide:zap" to="/guides/stop-spam-fast" description="Disable one alias or tighten protection without changing your inbox address."}
  ::
  ::card{title="Vendor migrations" icon="lucide:shuffle" to="/guides/vendor-migrations" description="Move accounts like Amazon/Netflix to vendor-specific aliases."}
  ::
  ::card{title="What not to alias" icon="lucide:ban" to="/guides/what-not-to-alias" description="Gmail is your mailbox; use aliases for everything else."}
  ::
  ::card{title="An email didn’t arrive" icon="lucide:mail-x" to="/troubleshooting/email-didnt-arrive" description="Fast checklist for missing mail."}
  ::
  ::card{title="Data and privacy" icon="lucide:lock" to="/reference/data-and-privacy" description="What’s stored, what isn’t, and why."}
  ::
::
