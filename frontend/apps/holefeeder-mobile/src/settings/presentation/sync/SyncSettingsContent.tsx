import { File, Paths } from 'expo-file-system';
import * as Sharing from 'expo-sharing';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { DEFAULT_SYNC_INFO } from '@/settings/core/sync-info';
import { useSyncInfo } from '@/settings/presentation/core/use-sync-info';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppErrorSheet } from '@/shared/presentation/components/native/AppErrorSheet';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldGroup } from '@/shared/presentation/components/native/AppFieldGroup';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppSwitch } from '@/shared/presentation/components/native/AppSwitch';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useMultipleWatches, withDefault } from '@/shared/presentation/core/use-multiple-watches';
import { useStyles } from '@/shared/theme/core/use-styles';
import { spacing, Theme } from '@/types/theme';

const createStyles = (theme: Theme) => ({
  scrollView: {
    flexGrow: 1,
    marginHorizontal: spacing.lg,
    marginBottom: spacing.lg,
  },
  container: {
    ...theme.styles.containers.center,
  },
  section: {
    ...theme.styles.containers.section,
    marginHorizontal: undefined,
  },
});

export function SyncSettingsContent() {
  const styles = useStyles(createStyles);
  const { t } = useTranslation();
  const syncInfoQuery = useSyncInfo();

  const { data, isLoading, errors } = useMultipleWatches({
    syncInfo: withDefault(() => syncInfoQuery, DEFAULT_SYNC_INFO),
  });

  if (isLoading || !data) {
    return (
      <AppNative style={styles.container}>
        <AppLoadingIndicator />
        <AppErrorSheet {...errors} />
      </AppNative>
    );
  }

  const { syncInfo } = data;

  return (
    <AppNative style={{ flex: 1 }}>
      <AppFieldGroup>
        <AppFieldSection title={t(tk.settings.syncSection.title)} style={styles.section}>
          <AppField label={t(tk.settings.syncSection.connected)} icon={AppIconMap.connected}>
            <AppSwitch value={syncInfo.connected} disabled />
          </AppField>
          <AppField label={t(tk.settings.syncSection.lastSync)} icon={AppIconMap.sync}>
            <AppText>{syncInfo.lastSyncedAt ? syncInfo.lastSyncedAt.toLocaleString() : t(tk.settings.syncSection.never)}</AppText>
          </AppField>
          <AppField label={t(tk.settings.syncSection.downloading)} icon={AppIconMap.download}>
            <AppSwitch value={syncInfo.dataFlowStatus.downloading} disabled />
          </AppField>
          <AppField label={t(tk.settings.syncSection.uploading)} icon={AppIconMap.upload}>
            <AppSwitch value={syncInfo.dataFlowStatus.uploading} disabled />
          </AppField>
          <AppField label={t(tk.settings.syncSection.outstanding)} icon={AppIconMap.uploadOutstanding}>
            <AppText>{syncInfo.dataMetrics.outstandingTransactions.toString()}</AppText>
          </AppField>
        </AppFieldSection>
        <AppFieldSection title={t(tk.settings.syncSection.syncedData)} style={styles.section}>
          <AppField label={t(tk.settings.syncSection.accounts)} icon={AppIconMap.accounts}>
            <AppText>{syncInfo.dataMetrics.accounts.toString()}</AppText>
          </AppField>
          <AppField label={t(tk.settings.syncSection.cashflows)} icon={AppIconMap.cashflow}>
            <AppText>{syncInfo.dataMetrics.cashflows.toString()}</AppText>
          </AppField>
          <AppField label={t(tk.settings.syncSection.categories)} icon={AppIconMap.category}>
            <AppText>{syncInfo.dataMetrics.categories.toString()}</AppText>
          </AppField>
          <AppField label={t(tk.settings.syncSection.storeItems)} icon={AppIconMap.storeItem}>
            <AppText>{syncInfo.dataMetrics.storeItems.toString()}</AppText>
          </AppField>
          <AppField label={t(tk.settings.syncSection.transactions)} icon={AppIconMap.purchase}>
            <AppText>{syncInfo.dataMetrics.transactions.toString()}</AppText>
          </AppField>
          <AppField label={t(tk.settings.syncSection.share)} icon={AppIconMap.share}>
            <AppButton
              label={t(tk.settings.syncSection.share)}
              icon={AppIconMap.share}
              onPress={async () => {
                if (!(await Sharing.isAvailableAsync())) return;
                // op-sqlite stores DBs in NSLibraryDirectory on iOS
                const file = new File(Paths.cache.parentDirectory, 'holefeeder.db');
                const tempFile = new File(Paths.cache, 'holefeeder_temp.db');
                if (tempFile.exists) tempFile.delete();
                await file.copy(tempFile);
                await Sharing.shareAsync(tempFile.uri, {
                  mimeType: 'application/x-sqlite3',
                  UTI: 'public.database',
                });
              }}
            />
          </AppField>
        </AppFieldSection>
      </AppFieldGroup>
    </AppNative>
  );
}
