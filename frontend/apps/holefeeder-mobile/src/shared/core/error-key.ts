import { tk } from '@/i18n/translations';

export const ErrorKey = {
  noInternetConnection: 'noInternetConnection',
  cannotReachServer: 'cannotReachServer',
  saveFailed: 'saveFailed',
} as const;

export type ErrorKey = (typeof ErrorKey)[keyof typeof ErrorKey];

export const tkErrorTitles: Record<ErrorKey, string> = {
  [ErrorKey.noInternetConnection]: tk.errors.noInternetConnection.title,
  [ErrorKey.cannotReachServer]: tk.errors.cannotReachServer.title,
  [ErrorKey.saveFailed]: tk.errors.saveFailed.title,
};
export const tkErrorMessages: Record<ErrorKey, string> = {
  [ErrorKey.noInternetConnection]: tk.errors.noInternetConnection.message,
  [ErrorKey.cannotReachServer]: tk.errors.cannotReachServer.message,
  [ErrorKey.saveFailed]: tk.errors.saveFailed.message,
};
