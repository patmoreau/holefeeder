import { tk } from '@holefeeder/shared/core';
import { useApiRefresh } from '@/shared/api/api-refresh';
import { AccountTypes } from '@/shared/domain/account-type';
import { translate } from '@/shared/i18n/translate';
import { AccountFlyout } from '@/shared/presentation/AccountFlyout';
import type { Notify } from '@/shared/presentation/DashboardToast';
import { useAddAccount } from '../api/use-add-account';

const today = () => new Date().toLocaleDateString('en-CA');

type AddAccountFlyoutProps = { open: boolean; onClose: () => void; notify: Notify };

export const AddAccountFlyout = ({ open, onClose, notify }: AddAccountFlyoutProps) => {
  const addAccount = useAddAccount();
  const { refresh } = useApiRefresh();

  const saved = () => {
    notify(translate(tk.accountEdit.added));
    refresh();
    onClose();
  };

  return (
    <AccountFlyout
      open={open}
      testID="add-account"
      title={translate(tk.accountEdit.addTitle)}
      submitLabel={translate(tk.accountEdit.add)}
      initial={{ type: AccountTypes.checking, name: '', openDate: today(), openBalance: 0, description: '', favorite: false }}
      onSubmit={addAccount}
      onSaved={saved}
      onClose={onClose}
    />
  );
};
