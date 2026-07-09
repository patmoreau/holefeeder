import * as Haptics from 'expo-haptics';
import { useTranslation } from 'react-i18next';
import { Account } from '@/accounts/core/account';
import { PurchaseFormError, usePurchaseForm } from '@/flows/presentation/purchase/core/use-purchase-form';
import { AccountField } from '@/flows/presentation/shared/components/AccountField';
import { DescriptionField } from '@/flows/presentation/shared/components/DescriptionField';
import { tk } from '@/i18n/translations';
import { DateField } from '@/shared/presentation/components/fields/DateField';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';

const tkErrors: Record<PurchaseFormError, string> = {
  [PurchaseFormError.accountRequired]: tk.purchase.errors.accountRequired,
  [PurchaseFormError.amountRequired]: tk.purchase.errors.amountRequired,
  [PurchaseFormError.categoryRequired]: tk.purchase.errors.categoryRequired,
  [PurchaseFormError.sameAccount]: tk.purchase.errors.sameAccount,
};

type Props = {
  accounts: Account[];
};

export const TransferSection = ({ accounts }: Props) => {
  const { t } = useTranslation();
  const { formData, updateFormField, errors, clearErrors } = usePurchaseForm();

  const fieldError = (error: PurchaseFormError | undefined) => (error ? t(tkErrors[error]) : undefined);

  const updateSourceAccount = (account: Account) => {
    updateFormField('sourceAccount', account);
    if (errors.targetAccount) {
      clearErrors('targetAccount');
    }
  };

  const updateTargetAccount = (account: Account) => {
    updateFormField('targetAccount', account);
    if (account.id === formData.sourceAccount?.id) {
      Haptics.notificationAsync(Haptics.NotificationFeedbackType.Error).then();
    }
  };

  const updateDescription = (value: string) => updateFormField('description', value);

  return (
    <AppFieldSection>
      <DateField
        label={t(tk.purchase.transferSection.date)}
        selectedDate={formData.date}
        onDateSelected={(date) => updateFormField('date', date)}
      />
      <AccountField
        label={t(tk.purchase.transferSection.accountFrom)}
        accounts={accounts}
        selectedAccount={formData.sourceAccount}
        onSelectAccount={updateSourceAccount}
      />
      <AccountField
        label={t(tk.purchase.transferSection.accountTo)}
        accounts={accounts}
        selectedAccount={formData.targetAccount}
        onSelectAccount={updateTargetAccount}
        error={fieldError(errors.targetAccount)}
      />
      <DescriptionField description={formData.description} onDescriptionChange={updateDescription} />
    </AppFieldSection>
  );
};
