import { Resource } from 'i18next';
import { DashboardTranslations } from '@/dashboard/presentation/core/useDashboardTranslations';
import { en as enCaDashboardTranslations } from '@/dashboard/presentation/locals/en-CA/translations';
import { fr as frCaDashboardTranslations } from '@/dashboard/presentation/locals/fr-CA/translations';

export const dashboardTranslationsResources: Resource = {
  en: {
    [DashboardTranslations]: enCaDashboardTranslations,
  },
  fr: {
    [DashboardTranslations]: frCaDashboardTranslations,
  },
};
