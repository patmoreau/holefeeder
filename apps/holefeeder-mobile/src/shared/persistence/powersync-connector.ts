import { AbstractPowerSyncDatabase, type PowerSyncBackendConnector } from '@powersync/common';
import { HolefeederConfig } from '@/config/holefeeder-config';
import { syncApi } from '@/shared/api/sync-api';
import { AuthenticationState } from '@/shared/auth/core/autentication-state';
import { Logger } from '@/shared/core/logger/logger';

const logger = Logger.create('powersync-connector');

export const PowerSyncConnector = (authenticationState: AuthenticationState, config: HolefeederConfig): PowerSyncBackendConnector => {
  const fetchCredentials = async () => {
    const token = await authenticationState.getToken();
    if (!token) {
      return null;
    }
    return {
      endpoint: config.powersyncConfig.url,
      token: token.token,
      expiresAt: token.expiresAt ? new Date(token.expiresAt) : undefined,
    };
  };

  const uploadData = async (database: AbstractPowerSyncDatabase): Promise<void> => {
    const transaction = await database.getNextCrudTransaction();
    if (!transaction) {
      return;
    }

    if (!authenticationState.user) {
      logger.info('No user logged in, skipping upload.');
      return;
    }

    try {
      const result = await syncApi(authenticationState, config.apiConfig).upload({
        transactionId: transaction.transactionId ? Number(transaction.transactionId) : undefined,
        operations: transaction.crud,
      });

      if (result.isFailure) {
        logger.error('Failed to upload data:', result.errors);
        return;
      }

      await transaction.complete();
    } catch (error) {
      logger.error('Error uploading data:', error);
      throw error;
    }
  };

  return { fetchCredentials: fetchCredentials, uploadData: uploadData };
};
