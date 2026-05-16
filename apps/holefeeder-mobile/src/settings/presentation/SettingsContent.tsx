import { BudgetSection } from '@/settings/presentation/BudgetSection';
import { DisplaySection } from '@/settings/presentation/DisplaySection';
import { ProfileSection } from '@/settings/presentation/ProfileSection';
import { SyncSection } from '@/settings/presentation/SyncSection';
import { TestSection } from '@/settings/presentation/TestSection';
import { AppForm } from '@/shared/presentation/AppForm';

export const SettingsContent = () => {
  return (
    <AppForm>
      <ProfileSection />
      <BudgetSection />
      <DisplaySection />
      {__DEV__ && <TestSection />}
      <SyncSection />
    </AppForm>
  );
};
