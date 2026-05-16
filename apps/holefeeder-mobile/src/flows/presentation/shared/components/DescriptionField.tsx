import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppField } from '@/shared/presentation/AppField';
import { AppTextInput } from '@/shared/presentation/components/AppTextInput';
import { AppIcons } from '@/shared/presentation/icons';

type Props = {
  description: string;
  onDescriptionChange: (description: string) => void;
  error?: string;
};

export const DescriptionField = ({ description, onDescriptionChange, error }: Props) => {
  const { t } = useTranslation();

  return (
    <AppField icon={AppIcons.description} error={error}>
      <AppTextInput placeholder={t(tk.purchase.basicSection.description)} value={description} onChangeText={onDescriptionChange} />
    </AppField>
  );
};
