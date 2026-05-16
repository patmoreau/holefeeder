import { useTranslation } from 'react-i18next';

export const DashboardTranslations = 'DashboardTranslations';

export const useDashboardTranslations = () => {
  return useTranslation(DashboardTranslations);
};
