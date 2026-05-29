import { Account } from '@/accounts/core/account';
import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppField } from '@/shared/presentation/components/app/AppField';
import { AppPicker } from '@/shared/presentation/components/app/AppPicker';

type Props = {
  label: string;
  accounts: Account[];
  selectedAccount: Account;
  onSelectAccount: (account: Account) => void;
  error?: string;
};

export function AccountField({ label, accounts, selectedAccount, onSelectAccount, error }: Props) {
  return (
    <AppField label={label} icon={AppIconMap.account} error={error}>
      <AppPicker
        options={accounts}
        selectedOption={selectedAccount}
        onSelectOption={onSelectAccount}
        onOptionLabel={(account) => account.name}
      />
    </AppField>
  );
}
