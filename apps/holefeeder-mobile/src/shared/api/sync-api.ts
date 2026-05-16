import { CrudEntry } from '@powersync/common';
import { ApiClient } from '@/shared/api/api-client';
import { ApiConfig } from '@/shared/api/api-config';
import { AuthenticationState } from '@/shared/auth/core/autentication-state';
import { Result } from '@/shared/core/result';

type SyncUploadApi = {
  transaction_id?: number;
  operations: CrudEntry[];
};

export const syncApi = (authenticationState: AuthenticationState, apiConfig: ApiConfig) => {
  const api = ApiClient(authenticationState, apiConfig);

  const upload = ({ transactionId, operations }: { transactionId?: number; operations: CrudEntry[] }): Promise<Result<void>> => {
    const payload: SyncUploadApi = {
      transaction_id: transactionId,
      operations: operations,
    };
    return api.post<SyncUploadApi, void>('/api/v2/sync/powersync', payload);
  };

  return {
    upload,
  };
};
