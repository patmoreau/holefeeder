export type BillsDueBadgeState = {
  enabled: boolean;
  denied: boolean;
  setEnabled: (value: boolean) => Promise<void>;
  openSettings: () => Promise<void>;
};
