import { AccountsRepositoryInMemory } from '@/accounts/core/__tests__/accounts-repository-for-test';
import { DashboardRepositoryInMemory } from '@/dashboard/__tests__/dashboard-repository-in-memory';
import { CategoriesRepositoryInMemory } from '@/flows/core/categories/__tests__/categories-repository-for-test';
import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { SettingsRepositoryInMemory } from '@/settings/core/__tests__/settings-repository-for-test';
import { StoreItemsRepositoryInMemory } from '@/shared/__tests__/store-items-repository-for-test';
import { RepositoriesState } from '@/shared/repositories/core/repositories-state';
import { InsightsRepositoryInMemory } from '@/statistics/__tests__/insights-repository-in-memory';

const defaultRepositoriesState: RepositoriesState = {
  accountRepository: AccountsRepositoryInMemory(),
  categoryRepository: CategoriesRepositoryInMemory(),
  dashboardRepository: DashboardRepositoryInMemory(),
  flowRepository: FlowsRepositoryInMemory(),
  insightsRepository: InsightsRepositoryInMemory(),
  settingRepository: SettingsRepositoryInMemory(),
  storeItemRepository: StoreItemsRepositoryInMemory(),
};

export const aRepositoriesState = (overrides?: Partial<RepositoriesState>): RepositoriesState => ({
  ...defaultRepositoriesState,
  ...overrides,
});
