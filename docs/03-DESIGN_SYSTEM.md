# Korner — Design System & UX Rules

> Visual and interaction rules. Every component must follow these tokens and states.
> No hex colour or arbitrary spacing inside components — only tokens and the spacing scale.

---

## 1. Product context (for design decisions)

- **Users:** 18–40, Egypt, mostly mid-range Android phones, arriving from Facebook/Instagram ads or WhatsApp links. Second user: the admin working from laptop and phone.
- **Personas:** Omar (21, student, wants a T-shirt fast, worried about size) → clear size guide, guest checkout.
  Mona (29, buying a perfume gift) → authenticity info, delivery date by day, easy card payment.
  Ahmed (36, repeat buyer) → Google sign-in, saved address (P2 buy-again).
  The admin (packs ~50 orders/day) → dense order list, status tabs, fast status change, no wrong size shipped.
- **Goals in order:** find product → confirm size & final price → pay in < 2 minutes → know where the order is → return easily.
- **Main flow:** Ad/search → Product → (pick size) → Cart drawer → Checkout (one page) → Paymob → Result → Tracking.
- **Information architecture:** Home → Men / Women / Perfumes / Offers / Account / Help. Help = Track order, Shipping, Returns, Size guide, FAQ, Contact. Any product ≤ 3 taps from home.
- **Measurable UX:** product → order placed ≤ 4 steps for a guest; checkout ≤ 8 fields; final price incl. shipping visible before checkout; every action gives visible feedback within 100 ms.
- **Visual references (patterns, not looks):** COS / Uniqlo (white space, product image is the hero); Zara / H&M product page (desktop: gallery at the start side + sticky info; mobile: swipe gallery); Noon / Amazon.eg mobile (sticky bottom add-to-cart, filters in a bottom sheet); Baymard checkout research.

---

## 2. Theme tokens (`frontend/app/app.css`)

Base: tweakcn "Twitter" theme (`npx shadcn@latest add https://tweakcn.com/r/themes/twitter.json`) with **accessibility fixes**
(marked `/* a11y */`) and three added tokens (`--brand`, `--link`, `--success`, `--warning`). Use this file as-is.

```css
@import "tailwindcss";
@import "@fontsource/open-sans/400.css";
@import "@fontsource/open-sans/500.css";
@import "@fontsource/open-sans/600.css";
@import "@fontsource/noto-sans-arabic/400.css";
@import "@fontsource/noto-sans-arabic/500.css";
@import "@fontsource/noto-sans-arabic/600.css";

@custom-variant dark (&:is(.dark *));

:root {
  --background: oklch(1.0000 0 0);
  --foreground: oklch(0.1884 0.0128 248.5103);
  --card: oklch(0.9784 0.0011 197.1387);
  --card-foreground: oklch(0.1884 0.0128 248.5103);
  --popover: oklch(1.0000 0 0);
  --popover-foreground: oklch(0.1884 0.0128 248.5103);
  --primary: oklch(0.55 0.1606 244.9955);            /* a11y: was 0.6723 → white text 2.94:1, now 4.82:1 (#0077BC) */
  --primary-foreground: oklch(1.0000 0 0);
  --secondary: oklch(0.1884 0.0128 248.5103);
  --secondary-foreground: oklch(1.0000 0 0);
  --muted: oklch(0.9222 0.0013 286.3737);
  --muted-foreground: oklch(0.1884 0.0128 248.5103);
  --accent: oklch(0.9392 0.0166 250.8453);
  --accent-foreground: oklch(0.51 0.1606 244.9955);   /* a11y: 2.46:1 → 4.79:1 on --accent */
  --destructive: oklch(0.57 0.2376 25.7658);           /* a11y: white text 4.11:1 → 5.04:1 */
  --destructive-foreground: oklch(1.0000 0 0);
  --border: oklch(0.9317 0.0118 231.6594);             /* decorative only — never the only border of an input */
  --input: oklch(0.63 0.0118 231.6594);                /* a11y: input border was 1.05:1 → 3.49:1 */
  --ring: oklch(0.55 0.1606 244.9955);                 /* a11y */
  --brand: oklch(0.6723 0.1606 244.9955);              /* #1E9DF1 logo/icons/illustrations; NEVER under or as body text */
  --link: oklch(0.55 0.1606 244.9955);
  --success: oklch(0.5273 0.1372 150.0690);            /* #15803D */
  --warning: oklch(0.5553 0.1455 48.9980);             /* #B45309 */
  --chart-1: oklch(0.6723 0.1606 244.9955);
  --chart-2: oklch(0.6907 0.1554 160.3454);
  --chart-3: oklch(0.8214 0.1600 82.5337);
  --chart-4: oklch(0.7064 0.1822 151.7125);
  --chart-5: oklch(0.5919 0.2186 10.5826);
  --sidebar: oklch(0.9784 0.0011 197.1387);
  --sidebar-foreground: oklch(0.1884 0.0128 248.5103);
  --sidebar-primary: oklch(0.55 0.1606 244.9955);      /* a11y */
  --sidebar-primary-foreground: oklch(1.0000 0 0);
  --sidebar-accent: oklch(0.9392 0.0166 250.8453);
  --sidebar-accent-foreground: oklch(0.51 0.1606 244.9955);
  --sidebar-border: oklch(0.9271 0.0101 238.5177);
  --sidebar-ring: oklch(0.55 0.1606 244.9955);
  --font-sans: "Open Sans", "Noto Sans Arabic", sans-serif;
  --font-serif: Georgia, serif;
  --font-mono: Menlo, monospace;
  --radius: 1.3rem;
  --shadow-color: rgba(29,161,242,0.15);
  --shadow-2xs: 0px 2px 0px 0px hsl(202.8169 89.1213% 53.1373% / 0.00);
  --shadow-xs: 0px 2px 0px 0px hsl(202.8169 89.1213% 53.1373% / 0.00);
  --shadow-sm: 0px 2px 0px 0px hsl(202.8169 89.1213% 53.1373% / 0.00), 0px 1px 2px -1px hsl(202.8169 89.1213% 53.1373% / 0.00);
  --shadow: 0px 2px 0px 0px hsl(202.8169 89.1213% 53.1373% / 0.00), 0px 1px 2px -1px hsl(202.8169 89.1213% 53.1373% / 0.00);
  --shadow-md: 0px 2px 0px 0px hsl(202.8169 89.1213% 53.1373% / 0.00), 0px 2px 4px -1px hsl(202.8169 89.1213% 53.1373% / 0.00);
  --shadow-lg: 0px 2px 0px 0px hsl(202.8169 89.1213% 53.1373% / 0.00), 0px 4px 6px -1px hsl(202.8169 89.1213% 53.1373% / 0.00);
  --shadow-xl: 0px 2px 0px 0px hsl(202.8169 89.1213% 53.1373% / 0.00), 0px 8px 10px -1px hsl(202.8169 89.1213% 53.1373% / 0.00);
  --shadow-2xl: 0px 2px 0px 0px hsl(202.8169 89.1213% 53.1373% / 0.00);
  --tracking-normal: 0em;
  --spacing: 0.25rem;
}

:root:lang(ar) {
  --font-sans: "Noto Sans Arabic", "Open Sans", sans-serif;
}

.dark {
  --background: oklch(0 0 0);
  --foreground: oklch(0.9328 0.0025 228.7857);
  --card: oklch(0.2097 0.0080 274.5332);
  --card-foreground: oklch(0.8853 0 0);
  --popover: oklch(0 0 0);
  --popover-foreground: oklch(0.9328 0.0025 228.7857);
  --primary: oklch(0.55 0.1606 244.9955);              /* a11y: white text 2.97:1 → 4.82:1 */
  --primary-foreground: oklch(1.0000 0 0);
  --secondary: oklch(0.9622 0.0035 219.5331);
  --secondary-foreground: oklch(0.1884 0.0128 248.5103);
  --muted: oklch(0.2090 0 0);
  --muted-foreground: oklch(0.64 0.0078 247.9662);     /* a11y: 3.88:1 on card → 5.28:1 */
  --accent: oklch(0.1928 0.0331 242.5459);
  --accent-foreground: oklch(0.6692 0.1607 245.0110);
  --destructive: oklch(0.57 0.2376 25.7658);           /* a11y */
  --destructive-foreground: oklch(1.0000 0 0);
  --border: oklch(0.2674 0.0047 248.0045);
  --input: oklch(0.52 0.0288 244.8244);                /* a11y: 1.56:1 → 3.83:1 */
  --ring: oklch(0.6818 0.1584 243.3540);
  --brand: oklch(0.6692 0.1607 245.0110);
  --link: oklch(0.6692 0.1607 245.0110);               /* 7.06:1 on black */
  --success: oklch(0.8003 0.1821 151.7110);            /* #4ADE80 */
  --warning: oklch(0.8369 0.1644 84.4286);             /* #FBBF24 */
  --chart-1: oklch(0.6723 0.1606 244.9955);
  --chart-2: oklch(0.6907 0.1554 160.3454);
  --chart-3: oklch(0.8214 0.1600 82.5337);
  --chart-4: oklch(0.7064 0.1822 151.7125);
  --chart-5: oklch(0.5919 0.2186 10.5826);
  --sidebar: oklch(0.2097 0.0080 274.5332);
  --sidebar-foreground: oklch(0.8853 0 0);
  --sidebar-primary: oklch(0.55 0.1606 244.9955);      /* a11y */
  --sidebar-primary-foreground: oklch(1.0000 0 0);
  --sidebar-accent: oklch(0.1928 0.0331 242.5459);
  --sidebar-accent-foreground: oklch(0.6692 0.1607 245.0110);
  --sidebar-border: oklch(0.3795 0.0220 240.5943);
  --sidebar-ring: oklch(0.6818 0.1584 243.3540);
  --shadow-color: rgba(29,161,242,0.25);
}

@theme inline {
  --color-background: var(--background);
  --color-foreground: var(--foreground);
  --color-card: var(--card);
  --color-card-foreground: var(--card-foreground);
  --color-popover: var(--popover);
  --color-popover-foreground: var(--popover-foreground);
  --color-primary: var(--primary);
  --color-primary-foreground: var(--primary-foreground);
  --color-secondary: var(--secondary);
  --color-secondary-foreground: var(--secondary-foreground);
  --color-muted: var(--muted);
  --color-muted-foreground: var(--muted-foreground);
  --color-accent: var(--accent);
  --color-accent-foreground: var(--accent-foreground);
  --color-destructive: var(--destructive);
  --color-destructive-foreground: var(--destructive-foreground);
  --color-border: var(--border);
  --color-input: var(--input);
  --color-ring: var(--ring);
  --color-brand: var(--brand);
  --color-link: var(--link);
  --color-success: var(--success);
  --color-warning: var(--warning);
  --color-chart-1: var(--chart-1);
  --color-chart-2: var(--chart-2);
  --color-chart-3: var(--chart-3);
  --color-chart-4: var(--chart-4);
  --color-chart-5: var(--chart-5);
  --color-sidebar: var(--sidebar);
  --color-sidebar-foreground: var(--sidebar-foreground);
  --color-sidebar-primary: var(--sidebar-primary);
  --color-sidebar-primary-foreground: var(--sidebar-primary-foreground);
  --color-sidebar-accent: var(--sidebar-accent);
  --color-sidebar-accent-foreground: var(--sidebar-accent-foreground);
  --color-sidebar-border: var(--sidebar-border);
  --color-sidebar-ring: var(--sidebar-ring);

  --font-sans: var(--font-sans);
  --font-mono: var(--font-mono);
  --font-serif: var(--font-serif);

  --radius-sm: calc(var(--radius) - 4px);
  --radius-md: calc(var(--radius) - 2px);
  --radius-lg: var(--radius);
  --radius-xl: calc(var(--radius) + 4px);

  --shadow-2xs: var(--shadow-2xs);
  --shadow-xs: var(--shadow-xs);
  --shadow-sm: var(--shadow-sm);
  --shadow: var(--shadow);
  --shadow-md: var(--shadow-md);
  --shadow-lg: var(--shadow-lg);
  --shadow-xl: var(--shadow-xl);
  --shadow-2xl: var(--shadow-2xl);
}

@layer base {
  * { @apply border-border outline-ring/50; }
  body { @apply bg-background text-foreground font-sans antialiased; }
  :lang(ar) { letter-spacing: 0; }
  .tabular { font-variant-numeric: tabular-nums; }
}
```

**Verified contrast (WCAG 2.2 AA):** body text 18.5:1 (light) / 17.2:1 (dark); primary button 4.82:1; links 4.82:1 on
white, 4.53:1 on card, 7.06:1 in dark; accent text 4.79:1; destructive 5.04:1; input borders 3.49:1 / 3.83:1;
success & warning ≥ 5:1. `--brand` in light mode is 2.94:1 → decoration only.

**Colour rules:** colour never carries meaning alone (error = colour + icon + text; sold-out size = struck + "Sold out"
label). Blue (primary + brand) covers < 10 % of any screen — product photos colour the page. Dark mode only swaps tokens.
Dark mode is supported by tokens but the MVP ships **light mode only** (toggle in P2).

---

## 3. Typography

Open Sans (Latin) + Noto Sans Arabic, self-hosted via `@fontsource`, weights 400/500/600 only; preload 400 of the
page language. Prices use `tabular-nums`.

| Level | Mobile / Desktop (px) | Weight | Line height | Use |
| --- | --- | --- | --- | --- |
| Display | 32 / 48 | 600 | 1.2 | Home hero title |
| H1 | 24 / 32 | 600 | 1.3 | Product name, page title |
| H2 | 20 / 24 | 600 | 1.35 | Section titles |
| H3 | 18 / 20 | 500 | 1.4 | Sub-sections |
| Body | 16 / 16 | 400 | 1.7 | Text |
| Body-sm | 14 / 14 | 400 | 1.6 | Card meta, tables |
| Caption | 12 / 12 | 500 | 1.5 | Badges, helper text |
| Price | 18 / 20 | 600 | 1.2 | Product price |

Arabic letter-spacing always 0. One H1 per screen, no skipped levels. Readable line length ≤ ~70 characters.
Inputs ≥ 16 px (prevents iOS zoom).

---

## 4. Layout

- **Spacing scale (4 px base):** 0, 4, 8, 12, 16, 24, 32, 48, 64, 96 only.
- **Breakpoints:** mobile < 640 (4 cols, gutter 16, margin 16, 2 products/row) · `sm/md` 640–1023 (8 cols, 24, 24, 3/row) · `lg` 1024–1279 (12 cols, 24, 32, 4/row) · `xl` ≥ 1280 (12 cols, 32, auto, 4/row).
- **Containers:** pages max 1280 px; checkout 1040 px (form + summary); long text 720 px.
- **Shell:** sticky header (64 px desktop / 56 px mobile) → content → footer. Admin: 256 px sidebar + fluid content.
- **Direction:** everything aligned to `start`. Use logical utilities only (`ms-*`, `me-*`, `ps-*`, `pe-*`, `start-*`, `end-*`, `text-start`). Physical `ml/mr/pl/pr/left/right` are forbidden (lint rule). Directional icons flip in RTL.
- **Language switcher** in header and footer; switches to the same page in the other language.
- **Product images:** fixed 3:4 ratio everywhere (CLS 0).

## 5. Visual style

| Element | Rule |
| --- | --- |
| Radius | `rounded-md` (~19 px) buttons & inputs · `rounded-lg` (~21 px) cards & dialogs · `rounded-sm` (~17 px) badges · `rounded-full` avatars & colour swatches. Images inside cards follow the card radius |
| Elevation | Flat theme (shadows transparent). Depth by `--border` and background steps (`card`, `popover`). Overlays behind dialogs/drawers: black 50 % |
| Borders | 1 px. Focus ring 2 px `--ring`, 2 px offset, `:focus-visible` only; never `outline: none` without a replacement |
| Icons | Lucide only, stroke 1.5, sizes 16/20/24. Icon-only buttons need `aria-label` |
| Illustrations | Simple line art in text colour + `--brand`, only for empty and error states |
| Product photos | Same background (`--card`), same light & angle, 3:4, ≥ 1200 px wide; second image = worn/in use, shown on hover (desktop) |
| Motion | 150 ms (fast) / 250 ms (base), ease-out; disabled under `prefers-reduced-motion`. z-index: dropdown 50 · sticky 100 · drawer 200 · modal 300 · toast 400 |

## 6. Components (shadcn/Radix + `cva` variants)

| Component | Variants / sizes | Korner rules |
| --- | --- | --- |
| Button | `default`(primary) · `secondary` · `outline` · `ghost` · `destructive` · `link`; `sm` 36 · `md` 44 · `lg` 52 | One primary button per screen; labels are verbs ("Add to cart"); `sm` only in admin desktop |
| Product card | image, name (2 lines max), price, compare-at price, one badge, colour dots | Whole card is one link |
| Input | height 44, label above, helper text, error text | `inputMode="tel"` for phone, correct `autocomplete` on every field |
| Select / Combobox | governorate, area | Searchable when > 10 options |
| Size selector | custom radio group, tiles ≥ 44×44 | Sold-out tiles struck through (CAT-05); arrow-key navigation |
| Colour swatch | 32 px circle in a 44 px target | Colour name shown next to it |
| Checkbox / radio | 20 px in a 44 px target | Marketing consent unchecked by default |
| Dialog | confirmations, size guide | Focus trapped, Esc closes; never used for a checkout step |
| Drawer / Sheet | cart, mobile filters, mobile menu | Opens from the `end` side |
| Table (admin) | sticky header, numbers aligned end, 40 px compact rows | Becomes cards on mobile |
| Tabs | description / size guide | Accordion on mobile |
| Badge | sale, new, sold out, order status | Max one badge on a product image |
| Toast | success / error | Bottom on mobile; never for field errors |
| Pagination | storefront: "Load more" + `?page=` in URL; admin: numbered | No infinite scroll |
| Breadcrumbs | category & product | With JSON-LD |
| Header | logo (text wordmark "Korner" until a logo exists), categories, search (always visible), account, language, cart with count | Shrinks on scroll |
| Footer | help & policy links, payment method icons, contact & WhatsApp number | — |

## 7. Required states for every component

| State | Look | Example |
| --- | --- | --- |
| Default | base tokens | "Add to cart" in primary |
| Hover | background ±8 % in 150 ms, no layout shift | Product card shows 2nd image (desktop) |
| Focus | 2 px ring, 2 px offset | Keyboard through sizes |
| Active / selected | slight darken or scale 0.98; selected = 2 px primary border + check | Selected size |
| Disabled | 50 % opacity, `aria-disabled`, reason shown | "Publish" when a language is missing. "Add to cart" before choosing a size stays enabled and scrolls to sizes with "Choose a size" |
| Loading | button keeps width + spinner and blocks re-click; content uses skeletons of the final shape | "Processing…", product grid skeleton |
| Error | `destructive` border + icon + message that says how to fix; `aria-invalid` + `aria-describedby`; focus first invalid field | Phone format message (CHK-03) |
| Success | `success` colour + check, at the place of the action | "Added ✓" for 1 s, cart count bumps |
| Empty | illustration + sentence + one action | "Your cart is empty" + "Shop new arrivals" |

Commerce-specific states: sold out, low stock ("Only 3 left"), dropship lead time, price changed in cart, payment still
being confirmed, payment failed + retry. Loading rule: no spinner for < 300 ms; after 10 s show an explanation.
Every component has a story on the dev-only route `/dev/components` (both languages, all states).

## 8. UX principles (applied)

| Principle | Rule |
| --- | --- |
| Hierarchy | Product page above the fold on mobile: image → name & price → colour → size → add to cart; description below |
| Consistency | Same component & same word for the same thing ("Cart" / "السلة" everywhere) |
| Usability | Main flows tested with 5 real people before launch; the task "buy a size L T-shirt" must take < 2 minutes |
| Accessibility | WCAG 2.2 AA; full keyboard; screen reader labels; targets ≥ 44 px; correct `lang`/`dir` |
| Feedback | Optimistic cart updates with rollback; every action visible in ≤ 100 ms |
| Error prevention | Validate on blur; governorate is a select; size guide next to sizes; order summary before paying |
| Error recovery | Forms never cleared on error; failed payment returns to the same order; undo after removing a cart item |
| Progressive disclosure | Long description/specs in accordions; advanced filters collapsed |
| Cognitive load | ≤ 7 items in main menus; one decision per step; smart defaults |

## 9. Responsive behaviour

| Element | Mobile | Tablet | Desktop |
| --- | --- | --- | --- |
| Navigation | header (logo, search, cart) + drawer menu (incl. language) + bottom bar (Home, Categories, Cart, Account) | as mobile without bottom bar | horizontal menu + mega menu |
| Product grid | 2 columns | 3 | 4 |
| Filters | button → bottom sheet with "Show 23 results" | side sheet | sticky sidebar |
| Product page | full-width swipe gallery + sticky bottom bar (price + button) | 2 columns | 7/12 gallery + 5/12 sticky info |
| Cart | full-width drawer + `/cart` page | drawer | 420 px drawer |
| Checkout | one column, collapsed summary with total on top | one column | form + sticky summary |
| Admin tables | cards | key columns | full table |

Primary actions in the thumb zone; bottom bar respects `safe-area-inset-bottom`. Test at 360, 390, 768, 1280, 1440 in both languages; no horizontal scroll.

## 10. Page specifications

| Page | Goal | Primary CTA | Content order |
| --- | --- | --- | --- |
| Home | Send to a category/offer | Shop (hero) | hero → categories → new arrivals → best sellers → trust bar (secure payment, 14-day returns, delivery to all governorates) |
| Category | Find a product | Product card | title + count → active filter chips → grid → load more |
| Product | Decide | Add to cart | gallery → name & price → colour → size (+ size guide) → button → delivery & returns in 2 lines → description → specs |
| Cart | Review | Checkout | free-shipping bar → lines → total with shipping → button |
| Checkout | Pay without errors | Pay {total} | contact (name, phone, email) → address (governorate, area, address, landmark) → delivery date → payment method (card / wallet) → summary → consents → button |
| Payment result | Reassure | Track your order | status (paid / confirming / failed + retry) → order number → delivery date → summary |
| Track order | Know status | — | form (number + phone) → status timeline → tracking link |
| My orders | Know status | Order details | active orders → past orders; cancel while allowed |
| Admin orders | Process fast | Change status | status tabs with counts → filters (URL) → table: number, customer, governorate, total, method, status, date, actions |
| Admin product | Add correctly | Save | basics (AR + EN) → images → variant grid (size × colour: price, stock, SKU, type, lead time) → SEO → publish |

Forms: single column, related fields grouped in `fieldset` + `legend`, optional fields marked "(optional)", submit at the end.
Interactions: pinch-zoom on product images (mobile); filters update without full reload; Back restores filters and
scroll position.
