import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppField } from '@/shared/presentation/AppField';
import { AppSwitch } from '@/shared/presentation/components/AppSwitch';
import { AppIcons } from '@/shared/presentation/icons';

type Props = {
  hasCashflow: boolean;
  onHasCashflowChange: (hasCashflow: boolean) => void;
  error?: string;
};

export const HasCashflowField = ({ hasCashflow, onHasCashflowChange, error }: Props) => {
  const { t } = useTranslation();

  return (
    <AppField label={t(tk.purchase.cashflowSection.cashflow)} icon={AppIcons.cashflow} error={error}>
      <AppSwitch value={hasCashflow} onChange={onHasCashflowChange} />
    </AppField>
  );
};
