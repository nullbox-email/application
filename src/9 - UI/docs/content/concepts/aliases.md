---
title: Aliases
description: Disposable addresses you give to other services, not your real inbox.
icon: lucide:at-sign
---

An **alias** is a unique email address you hand out to a specific service or person.

Example:

- `amazon@<your-routing-key>.nullbox.email`

Mail sent to the alias is processed by Nullbox and then either forwarded, quarantined, or dropped based on your settings.

## Why aliases are useful

- **Stop spam instantly:** disable one alias instead of changing your real inbox address.
- **Spot leaks:** if only one vendor had an address, you know who leaked it.
- **Reduce tracking:** you don’t have to reuse the same email everywhere.

## Alias fields you should know

- **Alias name:** a label you can change later (for your own organization).
- **Email prefix:** the part before `@` (this becomes the address).

::alert{type="warning" title="Email prefix can’t be changed later" icon="lucide:triangle-alert"}
Choose the prefix carefully. If you need a different address later, create a new alias and migrate the service.
::

## Enabled vs disabled

- **Enabled:** mail can be forwarded/quarantined (based on your protection settings).
- **Disabled:** inbound mail to the alias is dropped.

Disabling an alias is the fastest way to stop unwanted mail, but it also blocks legitimate mail to that address.

## Created manually vs auto-created

Aliases can be created:

- manually, from the UI, or
- automatically on first receipt if your mailbox has auto-create enabled

## Related settings

Two other settings strongly affect alias behavior:

- **Quarantine and safety checks** vs **Direct passthrough**
- **Learning mode** vs **Active protection**

See [Sender protection](/concepts/sender-protection).

