import React from 'react';
import { SyncSettingsContent } from '@/settings/presentation/sync/SyncSettingsContent';
import { AppScreen } from '@/shared/presentation/AppScreen';

const SyncSettingsSheet = () => {
  return (
    <AppScreen>
      <SyncSettingsContent />
    </AppScreen>
  );
};

export default SyncSettingsSheet;
