const USER_CANCELLED = 'USER_CANCELLED';

// Backing out of the Auth0 browser sheet is a choice, not a failure. The library
// reports it on `type`; `code` and `name` are checked too so a version that moves it
// does not turn a shrug back into a red screen.
export const isUserCancellation = (error: unknown): boolean => {
  if (typeof error !== 'object' || error === null) {
    return false;
  }

  const { type, code, name } = error as { type?: unknown; code?: unknown; name?: unknown };

  return type === USER_CANCELLED || code === USER_CANCELLED || name === USER_CANCELLED;
};
