import { AsyncResult, Result } from '@holefeeder/shared/core';
import { useEffect, useRef, useState } from 'react';
import { useSyncStatus } from '@/settings/presentation/core/use-sync-status';
import { useRegistration } from '@/user-registration/presentation/RegistrationProvider';

export type OnboardingRegistrationState = {
  progress: AsyncResult<void>;
  ready: boolean;
  retry: () => void;
};

// How long to wait for the first sync before going in anyway. PowerSync resolves
// the bucket from user_identities via the token's subject, so a client that
// connected before that row existed may not receive a checkpoint at all — waiting
// without a cap strands the user on this screen forever. Going in early costs a
// briefly empty dashboard; the screens already handle having no data.
export const SyncWaitInMilliseconds = 10_000;

// Registers the caller, then holds the screen until their data has actually
// arrived. Advancing on the register response alone drops them into an app with no
// accounts and no categories, which reads as broken rather than new.
//
// Reporting readiness rather than opening the gate: onboarding continues with the
// budget period, and only its last step lets the caller into the app.
export const useOnboardingRegistration = (): OnboardingRegistrationState => {
  const { registration, register } = useRegistration();
  const { lastSyncedAt } = useSyncStatus();

  const [registeredAt, setRegisteredAt] = useState<Date | undefined>(undefined);
  const started = useRef(false);

  useEffect(() => {
    if (started.current) {
      return;
    }
    started.current = true;
    register();
  }, [register]);

  useEffect(() => {
    if (registration.isSuccess && !registeredAt) {
      setRegisteredAt(new Date());
    }
  }, [registration, registeredAt]);

  const [waitedLongEnough, setWaitedLongEnough] = useState(false);

  useEffect(() => {
    if (!registeredAt) {
      return;
    }
    const timeout = setTimeout(() => setWaitedLongEnough(true), SyncWaitInMilliseconds);
    return () => clearTimeout(timeout);
  }, [registeredAt]);

  // A sync that finished before registering says nothing about the rows it created,
  // so wait for one that landed after.
  const synced = !!registeredAt && !!lastSyncedAt && lastSyncedAt.getTime() >= registeredAt.getTime();
  const ready = !!registeredAt && (synced || waitedLongEnough);

  const retry = () => {
    setRegisteredAt(undefined);
    setWaitedLongEnough(false);
    register();
  };

  return {
    progress: registration.isFailure ? registration : Result.loading(),
    ready: ready,
    retry: retry,
  };
};
