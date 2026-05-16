import React, { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { PurchaseType } from '@/flows/presentation/purchase/core/purchase-form-data';
import { tk } from '@/i18n/translations';
import { AppView } from '@/shared/presentation/AppView';
import { AppPicker, PickerOption } from '@/shared/presentation/components/AppPicker';
import { useStyles } from '@/shared/theme/core/use-styles';

const tkTypes: Record<PurchaseType, string> = {
  [PurchaseType.expense]: tk.purchase.transactionTypeSection.expense,
  [PurchaseType.gain]: tk.purchase.transactionTypeSection.income,
  [PurchaseType.transfer]: tk.purchase.transactionTypeSection.transfer,
};

type PurchaseTypeTypeOption = PickerOption & {
  value: PurchaseType;
};

type Props = {
  selectedPurchaseType: PurchaseType | null;
  onSelectPurchaseType: (purchaseType: PurchaseType) => void;
};

const createStyles = () => ({
  container: {
    paddingHorizontal: 16,
    width: '100%' as const,
  },
});

export const PurchaseTransferSection = ({ selectedPurchaseType, onSelectPurchaseType }: Props) => {
  const { t } = useTranslation();
  const styles = useStyles(createStyles);

  const options = useMemo<PurchaseTypeTypeOption[]>(() => {
    const types = Object.values(PurchaseType) as PurchaseType[];
    return types.map((type) => ({
      id: type,
      value: type,
    }));
  }, []);

  const selectedOption = options.find((opt) => opt.value === selectedPurchaseType) || options[0];

  return (
    <AppView style={styles.container}>
      <AppPicker
        variant="segmented"
        options={options}
        selectedOption={selectedOption}
        onSelectOption={(option) => onSelectPurchaseType(option.value)}
        onOptionLabel={(option) => t(tkTypes[option.value])}
      />
    </AppView>
  );
};
