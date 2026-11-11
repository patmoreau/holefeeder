import { useApiQuery } from '@/shared/api/use-api-query';
import { AccountDetail } from '../domain/account-detail';

export const useAccountDetail = (accountId: string) => useApiQuery(`/api/v2/accounts/${accountId}/detail`, AccountDetail.valid);
