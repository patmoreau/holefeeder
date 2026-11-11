# Bills due widget

iOS home screen widget on the mobile app showing how many bills are past due and how
many are due today. Complements the [bills due badge](bills-due-badge.md), which shows
both combined on the app icon.

**Status (2026-11-10):** done. `expo-widgets` approved on 2026-11-10.

## Approach

Same data as the badge: `useUpcomingFlows()` lists every unpaid date from each
cashflow's effective date onward, so past-due dates are already in it.

- **Past due** on day D: unpaid flows dated before D.
- **Due today** on day D: unpaid flows dated D.

The counts change with the calendar, not only with the data. Whenever the upcoming
flows change, the signed-in user changes, or the app comes to the foreground, the app
publishes a WidgetKit timeline: one entry at 00:00 local today, and one on each date a
bill falls due and the day after it (when it moves from due today to past due). iOS
switches entries on its own, so the widget stays right while the app is not running.

## Decisions

1. **`expo-widgets`.** Official SDK 58 package. The layout is a `'widget'` component
   written with `@expo/ui/swift-ui`; its config plugin adds the `ExpoWidgetsTarget`
   extension at prebuild, sharing the App Group `group.com.drifterapps.holefeeder`
   with the app and the share extension.
2. **Sizes:** `systemSmall` (two counts) and `systemMedium` (counts and amounts).
3. **Independent of the badge switch.** Adding the widget is the opt-in; it needs no
   notification permission. Signed out, it asks to open the app.
4. **Tap opens the dashboard** (`holefeeder://`), which lists the upcoming bills.
5. **Strings are prepared in the app.** The widget runtime can't import i18n or
   formatters, so labels and formatted amounts travel in the timeline props.
6. **At most 64 entries**, matching the badge's cap.
7. **Safe logo (Penpot proposal 21B/C/D, 2026-11-10).** Small: counts on the left,
   a 32pt logo tile top-right. Medium: 24pt tile and "Bills" header over the two
   columns. Signed out: 44pt tile. The widget can't read the app's bundled assets,
   so `installWidgetLogo` copies `assets/images/widget-tile.png` (exported from the
   Penpot asset board, no glow) into the shared `widgetsDirectory` at launch and the
   props carry its file URI. Without it the widget simply shows no tile.
8. **Amounts split by direction (medium, Penpot proposal 22D).** Each column shows
   expenses with `↓` and gains with `↑`, each only when non-zero; when both are zero
   it shows the zero amount with no arrow. The counts still include both, like the
   badge. The label sits above the amounts, the count is centred on the amounts
   only, and the amounts are right-aligned with the arrow after the value so
   decimals and arrows line up. Labels stay on one line so French and English take
   the same height, and the content is centred vertically in the widget.
9. **`dashboard/widgets/` ring.** Holds the widget component and the adapter implementing
   `BillsDueWidgetPublisher`. It may import `@expo/ui` directly, like `modules/`.
   It sits in `dashboard/`, a composition slice, because the widget also reads the
   `summary/` read-model (decision 10).
10. **Spending and projected footer (medium, Penpot proposal 23A, 2026-11-10).**
    Under the bills, one line each: current spending for the budget period and the
    projected net flow, the same figures as the dashboard header (`projectedNetFlow`).
    Projected is green when positive, red when negative. Both are the same in every
    timeline entry: they change with transactions, not the calendar, so they refresh
    when the app publishes again.
11. **Spending widget (small, Penpot proposal 23C, 2026-11-10).** A second widget,
    `SpendingWidget`, `systemSmall` only: "Spending" with the logo tile, current
    spending in large type, the trend against the average (`↘` green under it, `↗`
    red over it, hidden until there is an average, like the dashboard), then the
    projected net flow. Published as a single snapshot, since nothing in it changes
    with the calendar. `HomeScreenWidgets` installs the logo once for both widgets.

## Limitations

- Upcoming flows only reach the end of the current budget period, so entries past it
  miss bills not yet listed. Opening the app refreshes the timeline. The badge has
  the same limitation.
- Spending and projected only refresh when the app runs. A transaction synced while
  the app is closed shows up the next time it is opened.
- The widget itself is not covered by Maestro: it lives on the home screen.
