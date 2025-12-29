---
title: Migrating vendors to aliases
description: Move logins and receipts off your personal inbox and onto vendor-specific aliases.
icon: lucide:shuffle
---

Vendor-specific aliases make it easy to:

- Stop spam by disabling a single alias
- See who leaked your address
- Keep your real mailbox address private

## The standard migration recipe

::steps
### 1) Create an alias for the vendor
Pick a clear name so you can recognize it later, for example:

- `amazon@your-domain.com`
- `netflix@your-domain.com`

### 2) Make sure the alias will accept the vendor’s first email
Some aliases can be configured to quarantine unknown senders.

During a migration, you want the *first* message (usually a verification email) to be delivered.

If your alias has sender filtering enabled, turn on **Learning Mode** (or **Direct Passthrough**) before you change the vendor email address.

### 3) Change the email address on the vendor account
In the vendor’s account settings, replace your old email with the new alias.

Most vendors will send a verification link/code to the new address.

### 4) Confirm the change
Open the verification message sent to the alias and complete the flow.

### 5) Tighten things back up (optional)
If you enabled Learning Mode / Passthrough temporarily, turn it back off once the vendor is verified and stable.
::

::alert{type="warning" title="If you don’t see the verification email" icon="lucide:triangle-alert"}
Make sure the alias is enabled, and check whether the alias is quarantining unknown senders. In the backend, you can also confirm the final decision via the `DeliveryAction` written by `ProcessEmailCommandHandler`.
::

## Starter vendor pages

::card-group{cols="2"}
  ::card{title="Amazon" icon="lucide:shopping-cart" to="/guides/vendor-migrations/amazon" description="Move receipts, delivery updates, and security emails to an alias."}
  ::
  ::card{title="Netflix" icon="lucide:clapperboard" to="/guides/vendor-migrations/netflix" description="Update your Netflix login email to a dedicated alias."}
  ::
  ::card{title="Hulu" icon="lucide:tv" to="/guides/vendor-migrations/hulu" description="Switch Hulu account email over to an alias."}
  ::
  ::card{title="Disney+" icon="lucide:film" to="/guides/vendor-migrations/disney-plus" description="Change your Disney+ email address to an alias."}
  ::
  ::card{title="Walmart" icon="lucide:store" to="/guides/vendor-migrations/walmart" description="Move order confirmations and account alerts to an alias."}
  ::
::

## Creating a new vendor page

When you add a new vendor, keep it consistent:

- Suggest a default alias name (`vendor@your-domain.com`)
- Explain where to find the “change email” setting
- Call out verification/recovery gotchas (2FA, email confirmation, sign-out risk)
