export const RegistrationStatuses = {
  registered: 'registered',
  notRegistered: 'not-registered',
} as const;

export type RegistrationStatus = (typeof RegistrationStatuses)[keyof typeof RegistrationStatuses];
