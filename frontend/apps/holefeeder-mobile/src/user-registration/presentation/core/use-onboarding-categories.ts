import { Money } from '@holefeeder/shared/core';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { CreateCategoryCommand } from '@/flows/core/categories/create/create-category-command';
import { CreateCategoryUseCase } from '@/flows/core/categories/create/create-category-use-case';
import { tk } from '@/i18n/translations';
import { CategoryTypes } from '@/shared/core/category-type';
import { useRepositories } from '@/shared/repositories/core/use-repositories';
import { SuggestedCategoryColors, SuggestedCategoryKey, SuggestedCategoryKeys } from '@/user-registration/core/suggested-categories';
import { useRegistration } from '@/user-registration/presentation/RegistrationProvider';

export type SuggestedCategoryChoice = {
  key: SuggestedCategoryKey;
  name: string;
  selected: boolean;
};

export type OnboardingCategoriesState = {
  choices: SuggestedCategoryChoice[];
  isSaving: boolean;
  failed: boolean;
  toggle: (key: SuggestedCategoryKey) => void;
  finish: () => Promise<void>;
  skip: () => void;
};

// The last step: whatever is chosen here is written through PowerSync like any other
// category the user creates later, and finishing opens the gate.
export const useOnboardingCategories = (): OnboardingCategoriesState => {
  const { categoryRepository } = useRepositories();
  const { recheck } = useRegistration();
  const { t } = useTranslation();

  const [selected, setSelected] = useState<SuggestedCategoryKey[]>([...SuggestedCategoryKeys]);
  const [isSaving, setIsSaving] = useState(false);
  const [failed, setFailed] = useState(false);

  const choices = SuggestedCategoryKeys.map((key) => ({
    key: key,
    name: t(tk.onboarding.suggestedCategories[key]),
    selected: selected.includes(key),
  }));

  const toggle = (key: SuggestedCategoryKey) =>
    setSelected((current) => (current.includes(key) ? current.filter((selectedKey) => selectedKey !== key) : [...current, key]));

  const finish = async () => {
    setIsSaving(true);
    setFailed(false);
    try {
      const useCase = CreateCategoryUseCase(categoryRepository);

      for (const choice of choices.filter((candidate) => candidate.selected)) {
        const command = CreateCategoryCommand.create({
          name: choice.name,
          type: CategoryTypes.expense,
          color: SuggestedCategoryColors[choice.key],
          budgetAmount: Money.valid(0),
          favorite: false,
        });
        if (command.isFailure || command.isLoading) {
          setFailed(true);
          return;
        }

        const result = await useCase.execute(command.value);
        if (result.isFailure) {
          // Stopping here leaves the categories created so far, which is harmless:
          // they are ordinary categories and the user can delete them.
          setFailed(true);
          return;
        }
      }

      recheck();
    } finally {
      setIsSaving(false);
    }
  };

  // Skipping is a real answer. The app works without these — the transfer categories
  // registration created are the only ones anything depends on.
  const skip = () => recheck();

  return {
    choices: choices,
    isSaving: isSaving,
    failed: failed,
    toggle: toggle,
    finish: finish,
    skip: skip,
  };
};
