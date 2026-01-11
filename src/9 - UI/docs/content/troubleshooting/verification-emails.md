---
title: Verification emails
description: Make sure sign-in codes and vendor confirmations don’t get stuck.
icon: lucide:key-round
---

Verification emails (codes, magic links, “confirm your new email”) are often the first message a vendor sends. If your alias is in **Active protection**, that first message may be quarantined.

## The safe way to do a migration

::steps
### Before you change the vendor email
On the alias you’ll use for the vendor, temporarily enable:

- **Learning mode**, or
- **Direct passthrough** (short window only)

This reduces the chance that the first verification message gets quarantined.

### Change the email on the vendor
Update the vendor account to use the alias address.

### Verify immediately
If the vendor sends a code or link, complete it right away.

### Tighten protection again
After you see a few real messages arriving, switch back to **Active protection** (and turn off passthrough if you used it).
::

## If you don’t see the verification email

1. Check the mailbox/alias activity feed for a quarantined or dropped decision.
2. If the alias is disabled, enable it and request “resend code.”
3. If the alias is in Active protection, switch to Learning mode temporarily and request “resend code.”

For vendor-specific walkthroughs, see [Vendor migrations](/guides/vendor-migrations).

