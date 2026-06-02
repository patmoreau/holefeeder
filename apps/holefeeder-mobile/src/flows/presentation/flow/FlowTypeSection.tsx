import React, { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { FlowType } from '@/flows/presentation/flow/core/flow-form-data';
import { tk } from '@/i18n/translations';
import { AppSegmentedMenu, PickerOption } from '@/shared/presentation/components/native/AppSegmentedMenu';

const tkTypes: Record<FlowType, string> = {
  [FlowType.expense]: tk.purchase.transactionTypeSection.expense,
  [FlowType.gain]: tk.purchase.transactionTypeSection.income,
};

type FlowTypeOption = PickerOption & {
  value: FlowType;
};

type Props = {
  selectedFlowType: FlowType | null;
  onSelectFlowType: (flowType: FlowType) => void;
};

export const FlowTypeSection = ({ selectedFlowType, onSelectFlowType }: Props) => {
  const { t } = useTranslation();

  const options = useMemo<FlowTypeOption[]>(() => {
    const types = Object.values(FlowType) as FlowType[];
    return types.map((type) => ({
      id: type,
      value: type,
    }));
  }, []);

  const selectedOption = options.find((opt) => opt.value === selectedFlowType) || options[0];

  return (
    <AppSegmentedMenu
      options={options}
      selectedOption={selectedOption}
      onSelectOption={(option) => onSelectFlowType(option.value)}
      onOptionLabel={(option) => t(tkTypes[option.value])}
    />
  );
};
