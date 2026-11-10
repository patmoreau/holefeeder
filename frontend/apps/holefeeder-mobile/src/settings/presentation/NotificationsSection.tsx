import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useBillsDueBadge } from '@/shared/bills-due-badge/core/use-bills-due-badge';
import { AppButtonVariant } from '@/shared/presentation/components/AppButtonVariant';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppFieldSectionFooter } from '@/shared/presentation/components/native/AppFieldSectionFooter';
import { AppSwitch } from '@/shared/presentation/components/native/AppSwitch';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

export function NotificationsSection() {
  const { t } = useTranslation();
  const { enabled, denied, setEnabled, openSettings } = useBillsDueBadge();

  return (
    <AppFieldSection title={t(tk.notificationsSection.title)}>
      <AppField label={t(tk.notificationsSection.billsDueBadge)} icon={AppIconMap.billsDueBadge}>
        <AppSwitch value={enabled} onChange={setEnabled} testID="settings-bills-due-badge-toggle" />
      </AppField>
      <AppFieldSectionFooter>
        {denied ? (
          <AppColumn alignment="start">
            <AppText variant="footnote">{t(tk.notificationsSection.billsDueBadgeDenied)}</AppText>
            <AppButton
              label={t(tk.notificationsSection.openSettings)}
              variant={AppButtonVariant.link}
              onPress={openSettings}
              testID="settings-bills-due-badge-open-settings"
            />
          </AppColumn>
        ) : (
          <AppText variant="footnote">{t(tk.notificationsSection.billsDueBadgeFooter)}</AppText>
        )}
      </AppFieldSectionFooter>
    </AppFieldSection>
  );
}
