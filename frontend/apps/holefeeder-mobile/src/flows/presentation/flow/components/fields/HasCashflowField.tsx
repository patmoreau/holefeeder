import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppSwitch } from '@/shared/presentation/components/native/AppSwitch';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

type Props = {
  hasCashflow: boolean;
  onHasCashflowChange: (hasCashflow: boolean) => void;
  error?: string;
};

export const HasCashflowField = ({ hasCashflow, onHasCashflowChange, error }: Props) => {
  const { t } = useTranslation();

  return (
    <AppField label={t(tk.purchase.cashflowSection.cashflow)} icon={AppIconMap.cashflow} error={error}>
      <AppSwitch value={hasCashflow} onChange={onHasCashflowChange} />
    </AppField>
  );
};
