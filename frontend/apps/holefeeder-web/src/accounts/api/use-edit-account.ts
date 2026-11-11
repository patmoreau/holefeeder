import { Result } from '@holefeeder/shared/core';
import { useApiCommand } from '@/shared/api/use-api-command';
import type { AccountDraft } from '@/shared/presentation/AccountFlyout';
import type { AccountDetail } from '../domain/account-detail';

export const useEditAccount = () => {
  const command = useApiCommand();

  return async (account: AccountDetail, draft: AccountDraft): Promise<Result<void>> => {
    const modified = await command('/api/v2/accounts/modify-account', {
      id: account.id,
      name: draft.name,
      type: draft.type,
      openDate: draft.openDate,
      openBalance: draft.openBalance.toFixed(2),
      description: draft.description,
    });
    if (!modified.isSuccess) return modified;

    if (draft.favorite !== account.favorite) {
      const favorite = await command('/api/v2/accounts/favorite-account', { id: account.id, isFavorite: draft.favorite });
      if (!favorite.isSuccess) return favorite;
    }

    if (draft.inactive && !account.inactive) return command('/api/v2/accounts/close-account', { id: account.id });

    return Result.success(undefined);
  };
};
