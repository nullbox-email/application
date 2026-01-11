---
title: Naming and organization
description: Practical naming schemes that scale, plus what can and can’t be changed later.
icon: lucide:tag
---

Your alias has two “names”:

- **Alias name**: the label you see in the UI (you can change this later).
- **Email prefix**: the part before `@` (this becomes the address and cannot be changed later).

## Simple naming schemes

Pick one scheme and stick to it:

- **Vendor-only:** `amazon@…`, `netflix@…`
- **Category + vendor:** `shop.amazon@…`, `stream.netflix@…`, `news.nyt@…`
- **Purpose-based:** `receipts.amazon@…`, `support.github@…`

## Allowed characters (email prefix)

Email prefixes support:

- letters and numbers
- dots (`.`), underscores (`_`), and dashes (`-`)

## Tips that prevent regrets

- Use dots to separate meaning (example: `shop.amazon`).
- Keep prefixes short; use the alias name field for a longer description.
- If you outgrow a prefix, create a new alias and migrate the vendor—don’t plan on “renaming” the address later.

## Advanced: “on-the-fly” aliases with auto-create

If your mailbox has **Automatically create new aliases** enabled, you can invent new prefixes on the fly and start receiving mail immediately (subject to account limits).

Example:

- `support.vendorname@<your-routing-key>.nullbox.email`
- `receipts.vendorname@<your-routing-key>.nullbox.email`

