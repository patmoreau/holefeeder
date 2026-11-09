# Bills due badge

App icon badge on the mobile app showing how many bills are due today.

**Status (2026-11-09):** draft, not started. Open questions below need answers
before step 1.

## Context

Upcoming bills are computed on the device from PowerSync data:
`WatchUpcomingFlowsUseCase` (`src/flows/core/flows/watch-upcoming/`) expands each
cashflow with `CashflowVariation.getUpcomingDates`, which returns every unpaid date
from the cashflow's effective date onward. Past-due unpaid dates are therefore
already in the list.

The app has no notification code and no notification library today.

## The problem

An iOS badge is a stored number. It does not recompute when the date changes. Setting
it only while the app is open leaves it wrong from midnight until the next launch, so
the count has to reach the icon while the app is not running.

## Approach

Scheduled local notifications, no backend.

Every time the upcoming flows change, and when the app comes to the foreground:

1. Set the badge to the count for today.
2. Cancel every pending badge notification.
3. Schedule one local notification per future day that has bills due, triggered at
   the start of that day, with `badge` set to that day's count.

iOS applies the badge when the notification fires, even if the app is killed. Paying
or deactivating a bill changes the upcoming flows, which reschedules everything, so
the count stays right.

iOS keeps at most 64 pending local notifications per app. One per day with bills
covers about two months, more than the upcoming window.

### Rejected

- **Badge set only while the app is open** — stale after midnight.
- **Background task (`expo-background-task`)** — iOS decides when it runs; cannot
  promise a correct badge at the start of the day.
- **Server push through APNs** — needs device tokens, an APNs key and a backend job
  for data the device already has.

## Layout

Following `docs/contributing/architecture-principles.md`, in the `flows` feature:

| Thing | Location |
|---|---|
| Count per day, schedule computation | `flows/core/flows/schedule-due-badges/` |
| `BadgeScheduler` port | `flows/core/flows/schedule-due-badges/` |
| `expo-notifications` adapter | new ring, name to agree (open question 4) |
| Trigger on upcoming changes and foreground | `flows/presentation/` hook, mounted in `src/app/(app)/_layout.tsx` |
| Permission prompt and toggle | `settings/` |

The use case takes upcoming flows and today's date and returns `{ today: number,
days: { date, count }[] }`, then hands it to the port. All date and count logic is
plain TypeScript, tested with Jest, no native code.

## Steps

Each step leaves `main` deployable. The feature stays off until the user turns it
on in Settings, so nothing prompts for permission before step 5.

1. **Add `expo-notifications`.** Dependency, config plugin in `app.config.ts`,
   prebuild, mock under `__mocks__/`. No behaviour change. Needs approval (new
   library).
2. **Spike on device.** Confirm a local notification with only `badge` set (no
   title, body or sound) is delivered silently and updates the badge on iOS 27, and
   that a badge-only permission request works. Throwaway branch; findings recorded
   here. The answer decides open question 2.
3. **Core use case, TDD.** Group upcoming flows by date, count today (plus past due
   per open question 1), build the schedule capped at 64 entries. Covers: no bills,
   several bills same day, paid bill excluded, past-due handling, date rollover,
   cap.
4. **Notifications adapter.** Implements `BadgeScheduler` with `setBadgeCountAsync`,
   `cancelAllScheduledNotificationsAsync` (or only ours, by identifier prefix) and
   `scheduleNotificationAsync` with a date trigger. Unit-tested against the mock.
5. **Settings toggle and permission.** "Badge for bills due" switch in Settings. Turning
   it on requests permission; denied leaves it off with a pointer to iOS Settings.
   Turning it off cancels everything and clears the badge. Stored with the other
   settings.
6. **Wire it up.** Hook that runs the use case on upcoming-flow changes and app
   foreground when the setting is on. Maestro flow under a new tag for the toggle;
   badge itself verified on device (simulator badge behaviour is not trustworthy).
7. **Clear on sign-out.** Logout cancels scheduled notifications and resets the
   badge to 0.

## Open questions

1. **What counts** — only bills due today, or also past-due unpaid ones? The
   upcoming list already includes past-due dates, so "due today or earlier" is the
   cheaper rule and probably the more useful one.
2. **Badge only, or also a banner** — "2 bills due today" as an alert at a chosen
   time would need alert permission too. Badge only is the smaller ask.
3. **Trigger time** — 00:00 local, or a morning hour? Only matters if a banner is
   added.
4. **Ring name** for the `expo-notifications` adapter: `notifications/`?
5. **Android** — badges depend on the launcher. In scope, or iOS only for now?
