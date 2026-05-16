import { Resource } from 'i18next';
import { dashboardTranslationsResources } from '@/dashboard/dashboard-translations';

export const translationResources: Resource = {
  en: {
    ...dashboardTranslationsResources.en,
  },
  fr: {
    ...dashboardTranslationsResources.fr,
  },
};
