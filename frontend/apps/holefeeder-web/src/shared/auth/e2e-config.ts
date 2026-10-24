export const E2eConfig = {
  isEnabled: (): boolean => import.meta.env.VITE_E2E === 'true' && !import.meta.env.PROD,
};
