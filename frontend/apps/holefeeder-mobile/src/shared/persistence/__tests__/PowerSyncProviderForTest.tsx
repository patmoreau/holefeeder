import { AbstractPowerSyncDatabase } from '@powersync/common';
import { PowerSyncContext } from '@powersync/react';
import React from 'react';
import { RepositoryProvider } from '@/shared/repositories/presentation/RepositoryContext';

export const PowerSyncProviderForTest = ({ children, database }: { children: React.ReactNode; database: AbstractPowerSyncDatabase }) => {
  return (
    <PowerSyncContext.Provider value={database}>
      <RepositoryProvider database={database}>{children}</RepositoryProvider>
    </PowerSyncContext.Provider>
  );
};
