---
title: Home
navigation: false
---

::hero
---
announcement:
  title: 'Start here'
  icon: 'lucide:mail'
  to: /how-it-works/process-email
actions:
  - name: How It Works
    to: /how-it-works/process-email
  - name: Guides
    variant: outline
    to: /guides/vendor-migrations
---

#title
Nullbox Email Docs

#description
How inbound email is processed, and how to use aliases safely.
::

::card-group{cols="2"}
  ::card{title="Inbound email processing" icon="lucide:mail" to="/how-it-works/process-email" description="The server-side decision pipeline (Forward/Drop/Quarantine)."}
  ::
  ::card{title="Vendor migrations" icon="lucide:shuffle" to="/guides/vendor-migrations" description="Move accounts like Amazon/Netflix to vendor-specific aliases."}
  ::
  ::card{title="What not to alias" icon="lucide:ban" to="/guides/what-not-to-alias" description="Gmail is your mailbox; use aliases for everything else."}
  ::
::
