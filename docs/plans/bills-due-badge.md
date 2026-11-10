# Bills due badge

App icon badge on the mobile app showing how many bills are due today or overdue.

**Status (2026-11-09):** done, all seven steps shipped. All questions answered and
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
2. ✅ **Spike on device.** Confirm a local notification with only `badge` set (no
   title, body or sound) is delivered silently and updates the badge on iOS 27, and
   that a badge-only permission request works. Done 2026-11-09 on an iPhone, iOS 27,
   from a throwaway branch (deleted):
   - `requestPermissionsAsync({ ios: { allowAlert: false, allowBadge: true,
     allowSound: false } })` showed the system prompt; the prompt's wording was not
     recorded.
   - `setBadgeCountAsync(3)` showed 3 at once.
   - A `TIME_INTERVAL` notification with `content: { badge: 7 }` changed the badge
     to 7 with the app in the background, and one with `badge: 11` changed it to 11
     after the app was force-quit. No banner and no sound either time.
   - Afterwards, Settings → Notifications → Holefeeder had Badges on, Lock Screen
     and Notification Center checked, and no Sounds switch. Those placements are
     the system defaults; with no title or body there is nothing to show in them.
   The approach holds; step 4 proceeds as written.
3. ✅ **Core use case, TDD.** Group upcoming flows by date, count today plus overdue,
   build the schedule capped at 64 entries. Done as `ScheduleDueBadgesUseCase` with
   the `BadgeScheduler` port: a day's badge is every unpaid bill dated on or before
   it, so overdue bills need no separate count. Covers: no bills,
   several bills same day, paid bill excluded, past-due handling, date rollover,
   cap.
4. ✅ **Notifications adapter.** Implements `BadgeScheduler` with `setBadgeCountAsync`,
   `cancelAllScheduledNotificationsAsync` (or only ours, by identifier prefix) and
   `scheduleNotificationAsync` with a date trigger. Adds the `expo-notifications`
   mock under `__mocks__/`; unit-tested against it. Done as
   `BadgeSchedulerWithExpoNotifications` in `flows/notifications/`: it cancels only
   pending notifications whose identifier starts with `due-badge-`, schedules each
   day at 00:00 local, and turns any exception into `schedule-badges-failed`. A
   `false` from `setBadgeCountAsync` (badges not allowed) is not a failure; step 5
   owns permission.
5. ✅ **Settings toggle and permission.** "Badge for bills due" switch in Settings. Turning
   it on requests badge permission; denied leaves it off with a pointer to iOS Settings.
   Turning it off cancels everything and clears the badge. Stored with the other
   settings. Done as designed in Penpot proposal 20A/20B: a Notifications section
   after Display, with a footer that switches to "Open Settings" when denied. The
   choice is kept on the device (MMKV, `app_settings_bills_due_badge`), since
   permission is per device; `BillsDueBadgeProvider` exposes it and re-checks the
   permission whenever the app becomes active. Turning it off only saves `false`:
   step 6 clears the badge when it sees the switch off. The provider lives in the
   settings feature and moves to `shared/` in step 6, when `flows` needs it.
6. ✅ **Wire it up.** Hook that runs the use case on upcoming-flow changes and app
   foreground when the setting is on. Maestro flow under a new tag for the toggle;
   badge itself verified on device (simulator badge behaviour is not trustworthy).
   Done as `useScheduleBillsDueBadge`, mounted by `BillsDueBadgeScheduler` in the
   signed-in layout. With the switch off it schedules an empty badge, which clears
   the icon and the pending notifications. It waits until the permission check has
   finished (`isLoading` on the switch state), so startup does not clear the badge
   before setting it. Rescheduling after a bill is added is not covered by a test:
   the test database adapter emits no table changes, so PowerSync watches never fire
   again in tests.
7. ✅ **Clear on sign-out.** Logout cancels scheduled notifications and resets the
   badge to 0. Done in `useScheduleBillsDueBadge`: with no signed-in user it schedules
   an empty badge, and it does the same when it stops running, whichever happens
   first when the signed-in screens go away. The switch itself is kept, since it
   belongs to the device like the permission.

## Decisions

1. **What counts** — bills due today and overdue unpaid ones. Decided 2026-11-09.
2. **Badge only** — no banner or sound; badge permission only. Decided 2026-11-09.
3. **Trigger time** — 00:00 local, since there is no banner to time. Follows from 2.
4. **Ring name** — `notifications/`, inside the `flows` feature. Decided 2026-11-09.
5. **Platform** — iOS only for now. Decided 2026-11-09.
