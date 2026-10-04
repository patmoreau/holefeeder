import { Stack } from 'expo-router';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppForm } from '@/shared/presentation/components/native/AppForm';
import { AppPinnedActions } from '@/shared/presentation/components/native/AppPinnedActions';
import { AppSwitch } from '@/shared/presentation/components/native/AppSwitch';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useOnboardingCategories } from '@/user-registration/presentation/core/use-onboarding-categories';

const CategoriesScreen = () => {
  const { choices, isSaving, failed, toggle, finish, skip } = useOnboardingCategories();
  const { t } = useTranslation();

  return (
    <AppScreen testID="onboarding-categories-screen">
      <Stack.Screen options={{ title: t(tk.onboarding.categoriesTitle) }} />
      <AppForm>
        <AppFieldSection title={t(tk.onboarding.categoriesSubtitle)}>
          {choices.map((choice) => (
            <AppField key={choice.key} icon={AppIconMap.category} label={choice.name}>
              <AppSwitch value={choice.selected} onChange={() => toggle(choice.key)} testID={`onboarding-category-${choice.key}-switch`} />
            </AppField>
          ))}
        </AppFieldSection>
        {failed && (
          <AppFieldSection>
            <AppField icon={AppIconMap.warning} label={t(tk.errors.saveFailed.title)}>
              <></>
            </AppField>
          </AppFieldSection>
        )}
      </AppForm>
      {/* Skipping is a real answer: nothing downstream depends on these. */}
      <AppPinnedActions
        primary={{ label: t(tk.onboarding.categoriesFinish), onPress: finish, testID: 'onboarding-categories-finish-button' }}
        secondary={{ label: t(tk.onboarding.categoriesSkip), onPress: skip, testID: 'onboarding-categories-skip-button' }}
        disabled={isSaving}
      />
    </AppScreen>
  );
};

export default CategoriesScreen;
