import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppSwitch } from '@/shared/presentation/components/native/AppSwitch';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useCombinedInsight } from './core/use-combined-insight';

export const CombinedInsightToggle = () => {
  const { t } = useTranslation();
  const { combined, setCombined } = useCombinedInsight();

  return (
    <AppFieldSection>
      <AppField label={t(tk.insights.combined)} icon={AppIconMap.combined}>
        <AppSwitch value={combined} onChange={setCombined} />
      </AppField>
    </AppFieldSection>
  );
};
