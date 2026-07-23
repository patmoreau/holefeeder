import { StoreItemsRepository } from '@holefeeder/shared/core';
import { AccountsRepository } from '@/accounts/core/accounts-repository';
import { DashboardRepository } from '@/dashboard/core/dashboard-repository';
import { CategoriesRepository } from '@/flows/core/categories/categories-repository';
import { FlowsRepository } from '@/flows/core/flows/flows-repository';
import { TagsRepository } from '@/flows/core/tags/tags-repository';
import { SettingRepository } from '@/settings/core/setting-repository';
import { InsightsRepository } from '@/statistics/core/insights-repository';

export type RepositoriesState = {
  accountRepository: AccountsRepository;
  categoryRepository: CategoriesRepository;
  dashboardRepository: DashboardRepository;
  flowRepository: FlowsRepository;
  insightsRepository: InsightsRepository;
  settingRepository: SettingRepository;
  storeItemRepository: StoreItemsRepository;
  tagRepository: TagsRepository;
};
