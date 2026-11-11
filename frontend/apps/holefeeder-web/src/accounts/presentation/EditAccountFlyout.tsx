import { tk } from '@holefeeder/shared/core';
import { useApiRefresh } from '@/shared/api/api-refresh';
import { translate } from '@/shared/i18n/translate';
import { AccountFlyout } from '@/shared/presentation/AccountFlyout';
import type { Notify } from '@/shared/presentation/DashboardToast';
import { useEditAccount } from '../api/use-edit-account';
import type { AccountDetail } from '../domain/account-detail';

type EditAccountFlyoutProps = { open: boolean; account: AccountDetail; onClose: () => void; notify: Notify };

export const EditAccountFlyout = ({ open, account, onClose, notify }: EditAccountFlyoutProps) => {
  const editAccount = useEditAccount();
  const { refresh } = useApiRefresh();

  const saved = () => {
    notify(translate(tk.accountEdit.saved));
    refresh();
    onClose();
  };

  return (
    <AccountFlyout
      open={open}
      testID="edit-account"
      showInactive
      title={translate(tk.accountEdit.title)}
      submitLabel={translate(tk.common.save)}
      initial={{
        type: account.type,
        name: account.name,
        openDate: account.openDate,
        openBalance: account.openBalance,
        description: account.description,
        favorite: account.favorite,
        inactive: account.inactive,
      }}
      onSubmit={(draft) => editAccount(account, draft)}
      onSaved={saved}
      onClose={onClose}
    />
  );
};
