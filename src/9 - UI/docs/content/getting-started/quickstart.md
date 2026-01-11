---
title: Quickstart
description: Create a mailbox, create your first alias, and send a test email.
icon: lucide:play
---

Nullbox works in two layers:

- A **Mailbox** is the destination (your real inbox) plus a private domain like `<your-routing-key>.nullbox.email`.
- An **Alias** is an address under that domain (like `amazon@<your-routing-key>.nullbox.email`) that you give to a service.

::steps
### Create a mailbox
In the Nullbox app, choose **Create mailbox**.

- Give it a name (example: “Personal”).
- Leave **Automatically create new aliases** on if you want to start using new addresses immediately.

### Create an alias
Open your mailbox, then choose **Create alias**.

- **Alias name**: what you’ll recognize later (example: “Amazon”).
- **Email prefix**: the part before `@` (example: `amazon`).

### Send a test email
From any email account, send a message to your alias (example: `amazon@<your-routing-key>.nullbox.email`).

You should see it arrive in your real inbox.

### Choose protection level
On the alias, you can toggle:

- **Quarantine and safety checks** vs **Direct passthrough**
- **Learning mode** vs **Active protection**

If you’re not sure, start with **Learning mode** while you set things up, then switch to **Active protection** for sensitive aliases.
::

## Next steps

- [How addresses are structured (mailboxes and routing keys)](/concepts/mailboxes)
- [Choosing alias protection settings](/getting-started/choosing-protection)
- [Naming and organizing aliases](/getting-started/naming-aliases)
- [Migrating vendor accounts safely](/guides/vendor-migrations)
- [What not to put behind an alias](/guides/what-not-to-alias)

::alert{type="info" title="Safe rule" icon="lucide:info"}
Your mailbox (Gmail/Outlook/iCloud/etc.) stays as-is. Give aliases to other services and forward mail into your mailbox.
::

