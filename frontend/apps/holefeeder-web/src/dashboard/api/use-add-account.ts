import { type AccountType, Result } from '@holefeeder/shared/core';
import { useApiCommand } from '@/shared/api/use-api-command';

export type NewAccountInput = {
  type: AccountType;
  name: string;
  openDate: string;
  openBalance: number;
  description: string;
  favorite: boolean;
};

export const useAddAccount = () => {
  const command = useApiCommand();

  return async (account: NewAccountInput): Promise<Result<void>> => {
    const opened = await command<object, { id: string }>('/api/v2/accounts/open-account', {
      type: account.type,
      name: account.name,
      openDate: account.openDate,
      openBalance: account.openBalance.toFixed(2),
      description: account.description,
    });
    if (!opened.isSuccess) return opened;
    if (!account.favorite) return Result.success(undefined);
    return command('/api/v2/accounts/favorite-account', { id: opened.value.id, isFavorite: true });
  };
};
