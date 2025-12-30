---
title: Amazon
description: Change the email on your Amazon account to a vendor-specific alias.
icon: streamline-logos:amazon-logo-solid
---

## Recommended alias

- `amazon@<your-routing-key>.nullbox.email`

## Before you start (1 minute)

- Make sure the alias is **enabled** and forwarding to your real inbox.
- If you use 2FA, keep your phone/authenticator nearby.
- If your alias filters unknown senders, turn on **Learning Mode** for this migration.

## Steps

::steps
### Open your Amazon login settings
Amazon website: **Account & Lists** → **Account** → **Login & security**

### Edit your email
Find **Email** and choose **Edit**.

Enter your alias (example: `amazon@<your-routing-key>.nullbox.email`) and save.

### Complete any security checks
Amazon may ask for:
- your password, and/or
- a one-time code, and/or
- a quick security challenge (captcha)

### Verify the email if prompted
If Amazon sends a verification link/code to the new email, open it and complete the verification.

### Watch the next few emails
Order confirmations, delivery updates, and security alerts should now arrive via the alias.
::

::alert{type="info" title="Tip" icon="lucide:info"}
If you have multiple Amazon regions/accounts, migrate them one at a time so you don’t get locked out mid-verification.
::
