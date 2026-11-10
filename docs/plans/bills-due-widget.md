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
8. **Amounts split by direction (medium).** Each column shows `↓` expenses and `↑`
   gains, each only when non-zero; when both are zero it shows the zero amount with
   no arrow. The counts still include both, like the badge.
9. **`widgets/` ring.** Holds the widget component and the adapter implementing
   `BillsDueWidgetPublisher`. It may import `@expo/ui` directly, like `modules/`.

## Limitations

- Upcoming flows only reach the end of the current budget period, so entries past it
  miss bills not yet listed. Opening the app refreshes the timeline. The badge has
  the same limitation.
- The widget itself is not covered by Maestro: it lives on the home screen.
