export type BillsDueBadgeState = {
  enabled: boolean;
  isLoading: boolean;
  denied: boolean;
  setEnabled: (value: boolean) => Promise<void>;
  openSettings: () => Promise<void>;
};
