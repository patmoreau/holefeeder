import { useState } from 'react';
import { BudgetSection } from '@/settings/presentation/BudgetSection';
import { DisplaySection } from '@/settings/presentation/DisplaySection';
import { ProfileSection } from '@/settings/presentation/ProfileSection';
import { SyncSection } from '@/settings/presentation/SyncSection';
import TestComponentsScreen from '@/settings/presentation/test/TestComponentsScreen';
import { TestSection } from '@/settings/presentation/TestSection';
import { AppForm } from '@/shared/presentation/components/app/AppForm';

export const SettingsContent = () => {
  const [show, setShow] = useState(false);
  return (
    <>
      <AppForm>
        <ProfileSection />
        <BudgetSection />
        <DisplaySection />
        {__DEV__ && <TestSection show={show} setShow={setShow} />}
        <SyncSection />
      </AppForm>
      <TestComponentsScreen show={show} setShow={setShow} />
    </>
  );
};
