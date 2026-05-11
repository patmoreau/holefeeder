import { AbstractPowerSyncDatabase } from '@powersync/common';
import React, { createContext, ReactNode, useMemo } from 'react';
import { AccountsRepositoryInPowersync } from '@/accounts/persistence/accounts-repository-in-powersync';
import { DashboardRepositoryInPowersync } from '@/dashboard/persistence/dashboard-repository-in-powersync';
import { CategoriesRepositoryInPowersync } from '@/flows/persistence/categories-repository-in-powersync';
import { FlowsRepositoryInPowersync } from '@/flows/persistence/flows-repository-in-powersync';
import { SettingRepositoryInPowersync } from '@/settings/persistence/setting-repository-in-powersync';
import { StoreItemsRepositoryInPowersync } from '@/shared/persistence/store-items-repository-in-powersync';
import { RepositoriesState } from '@/shared/repositories/core/repositories-state';

export const RepositoryContext = createContext<RepositoriesState | undefined>(undefined);

export const RepositoryProvider = ({ children, database }: { children: ReactNode; database: AbstractPowerSyncDatabase }) => {
  const repositories = useMemo<RepositoriesState>(
    () => ({
      accountRepository: AccountsRepositoryInPowersync(database),
      categoryRepository: CategoriesRepositoryInPowersync(database),
      dashboardRepository: DashboardRepositoryInPowersync(database),
      flowRepository: FlowsRepositoryInPowersync(database),
      settingRepository: SettingRepositoryInPowersync(database),
      storeItemRepository: StoreItemsRepositoryInPowersync(database),
    }),
    [database]
  );
  return <RepositoryContext.Provider value={repositories}>{children}</RepositoryContext.Provider>;
};
