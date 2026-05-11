import { File, Paths } from 'expo-file-system';
import * as Sharing from 'expo-sharing';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { ScrollView } from 'react-native';
import { tk } from '@/i18n/translations';
import { DEFAULT_SYNC_INFO } from '@/settings/core/sync-info';
import { useSyncInfo } from '@/settings/presentation/core/use-sync-info';
import { AppField } from '@/shared/presentation/AppField';
import { AppSection } from '@/shared/presentation/AppSection';
import { AppView } from '@/shared/presentation/AppView';
import { AppButton } from '@/shared/presentation/components/AppButton';
import { AppSwitch } from '@/shared/presentation/components/AppSwitch';
import { AppText } from '@/shared/presentation/components/AppText';
import { ErrorSheet } from '@/shared/presentation/components/ErrorSheet';
import { LoadingIndicator } from '@/shared/presentation/components/LoadingIndicator';
import { useMultipleWatches, withDefault } from '@/shared/presentation/core/use-multiple-watches';
import { AppIcons } from '@/shared/presentation/icons';
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
      <AppView style={styles.container}>
        <LoadingIndicator />
        <ErrorSheet {...errors} />
      </AppView>
    );
  }

  const { syncInfo } = data;

  return (
    <ScrollView style={styles.scrollView}>
      <AppSection title={t(tk.settings.syncSection.title)} style={styles.section}>
        <AppField label={t(tk.settings.syncSection.connected)} icon={AppIcons.connected}>
          <AppSwitch value={syncInfo.connected} readonly={true} />
        </AppField>
        <AppField label={t(tk.settings.syncSection.lastSync)} icon={AppIcons.sync}>
          <AppText>{syncInfo.lastSyncedAt ? syncInfo.lastSyncedAt.toLocaleString() : t(tk.settings.syncSection.never)}</AppText>
        </AppField>
        <AppField label={t(tk.settings.syncSection.downloading)} icon={AppIcons.download}>
          <AppSwitch value={syncInfo.dataFlowStatus.downloading} readonly={true} />
        </AppField>
        <AppField label={t(tk.settings.syncSection.uploading)} icon={AppIcons.upload}>
          <AppSwitch value={syncInfo.dataFlowStatus.uploading} readonly={true} />
        </AppField>
        <AppField label={t(tk.settings.syncSection.outstanding)} icon={AppIcons.uploadOutstanding}>
          <AppText>{syncInfo.dataMetrics.outstandingTransactions}</AppText>
        </AppField>
      </AppSection>
      <AppSection title={t(tk.settings.syncSection.syncedData)} style={styles.section}>
        <AppField label={t(tk.settings.syncSection.accounts)} icon={AppIcons.accounts}>
          <AppText>{syncInfo.dataMetrics.accounts}</AppText>
        </AppField>
        <AppField label={t(tk.settings.syncSection.cashflows)} icon={AppIcons.cashflow}>
          <AppText>{syncInfo.dataMetrics.cashflows}</AppText>
        </AppField>
        <AppField label={t(tk.settings.syncSection.categories)} icon={AppIcons.category}>
          <AppText>{syncInfo.dataMetrics.categories}</AppText>
        </AppField>
        <AppField label={t(tk.settings.syncSection.storeItems)} icon={AppIcons.storeItem}>
          <AppText>{syncInfo.dataMetrics.storeItems}</AppText>
        </AppField>
        <AppField label={t(tk.settings.syncSection.transactions)} icon={AppIcons.purchase}>
          <AppText>{syncInfo.dataMetrics.transactions}</AppText>
        </AppField>
        <AppField label={t(tk.settings.syncSection.share)} icon={AppIcons.share}>
          <AppButton
            label={t(tk.settings.syncSection.share)}
            icon={AppIcons.share}
            onPress={async () => {
              if (!(await Sharing.isAvailableAsync())) return;
              // op-sqlite stores DBs in NSLibraryDirectory on iOS
              const file = new File(Paths.cache.parentDirectory, 'holefeeder.db');
              const tempFile = new File(Paths.cache, 'holefeeder_temp.db');
              if (tempFile.exists) tempFile.delete();
              file.copy(tempFile);
              await Sharing.shareAsync(tempFile.uri, {
                mimeType: 'application/x-sqlite3',
                UTI: 'public.database',
              });
            }}
          />
        </AppField>
      </AppSection>
    </ScrollView>
  );
}
