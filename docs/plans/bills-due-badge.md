# Bills due badge

App icon badge on the mobile app showing how many bills are due today or overdue.

**Status (2026-11-09):** step 1 done, step 2 next. All questions answered and
`expo-notifications` approved on 2026-11-09.

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

The badge counts unpaid bills due today or earlier. It is a badge only: no title, body
or sound, and permission is requested for badges only.

Every time the upcoming flows change, and when the app comes to the foreground:

1. Set the badge to the count for today, overdue included.
2. Cancel every pending badge notification.
3. Schedule one local notification per future day that has bills due, triggered at
   00:00 local on that day, with `badge` set to that day's count plus the bills
   already overdue at scheduling time (they stay overdue until paid, and paying one
   reschedules).

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
| `expo-notifications` adapter | `flows/notifications/` |
| Trigger on upcoming changes and foreground | `flows/presentation/` hook, mounted in `src/app/(app)/_layout.tsx` |
| Permission prompt and toggle | `settings/` |

The use case takes upcoming flows and today's date and returns `{ today: number,
days: { date, count }[] }`, then hands it to the port. All date and count logic is
plain TypeScript, tested with Jest, no native code.

## Steps

Each step leaves `main` deployable. The feature stays off until the user turns it
on in Settings, so nothing prompts for permission before step 5.

1. ✅ **Add `expo-notifications`.** Dependency and config plugin in `app.config.ts`
   with `enableRemoteNotifications: false`, so the build carries no `aps-environment`
   entitlement and signing needs no Push Notifications capability. No behaviour
   change. The Jest mock waits for step 4, the first code that imports it.
2. **Spike on device.** Confirm a local notification with only `badge` set (no
   title, body or sound) is delivered silently and updates the badge on iOS 27, and
   that a badge-only permission request works. Throwaway branch; findings recorded
   here. If badge-only notifications are not silent, revisit before step 4.
3. **Core use case, TDD.** Group upcoming flows by date, count today plus overdue,
   build the schedule capped at 64 entries. Covers: no bills,
   several bills same day, paid bill excluded, past-due handling, date rollover,
   cap.
4. **Notifications adapter.** Implements `BadgeScheduler` with `setBadgeCountAsync`,
   `cancelAllScheduledNotificationsAsync` (or only ours, by identifier prefix) and
   `scheduleNotificationAsync` with a date trigger. Adds the `expo-notifications`
   mock under `__mocks__/`; unit-tested against it.
5. **Settings toggle and permission.** "Badge for bills due" switch in Settings. Turning
   it on requests badge permission; denied leaves it off with a pointer to iOS Settings.
   Turning it off cancels everything and clears the badge. Stored with the other
   settings.
6. **Wire it up.** Hook that runs the use case on upcoming-flow changes and app
   foreground when the setting is on. Maestro flow under a new tag for the toggle;
   badge itself verified on device (simulator badge behaviour is not trustworthy).
7. **Clear on sign-out.** Logout cancels scheduled notifications and resets the
   badge to 0.

## Decisions

1. **What counts** — bills due today and overdue unpaid ones. Decided 2026-11-09.
2. **Badge only** — no banner or sound; badge permission only. Decided 2026-11-09.
3. **Trigger time** — 00:00 local, since there is no banner to time. Follows from 2.
4. **Ring name** — `notifications/`, inside the `flows` feature. Decided 2026-11-09.
5. **Platform** — iOS only for now. Decided 2026-11-09.
