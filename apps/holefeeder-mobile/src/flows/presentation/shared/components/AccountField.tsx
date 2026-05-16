import React from 'react';
import { Account } from '@/accounts/core/account';
import { AppField } from '@/shared/presentation/AppField';
import { AppPicker } from '@/shared/presentation/components/AppPicker';
import { AppIcons } from '@/shared/presentation/icons';

type Props = {
  label: string;
  accounts: Account[];
  selectedAccount: Account;
  onSelectAccount: (account: Account) => void;
  error?: string;
};

export function AccountField({ label, accounts, selectedAccount, onSelectAccount, error }: Props) {
  return (
    <AppField label={label} icon={AppIcons.account} error={error}>
      <AppPicker
        options={accounts}
        selectedOption={selectedAccount}
        onSelectOption={onSelectAccount}
        onOptionLabel={(account) => account.name}
      />
    </AppField>
  );
}
