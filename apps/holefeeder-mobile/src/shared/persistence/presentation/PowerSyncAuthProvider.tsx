import { AbstractPowerSyncDatabase } from '@powersync/common';
import { PowerSyncContext } from '@powersync/react-native';
import { ReactNode, useEffect } from 'react';
import { HolefeederConfig } from '@/config/holefeeder-config';
import { useAuth } from '@/shared/auth/core/use-auth';
import { Logger } from '@/shared/core/logger/logger';
import { PowerSyncConnector } from '@/shared/persistence/powersync-connector';

const logger = Logger.create('PowerSyncAuthProvider');

export const PowerSyncAuthProvider = ({
  children,
  database,
  config,
}: {
  children: ReactNode;
  database: AbstractPowerSyncDatabase;
  config: HolefeederConfig;
}) => {
  const authenticationState = useAuth();

  useEffect(() => {
    if (authenticationState.isLoading && !authenticationState.user) {
      logger.debug('isLoading and no user, returning early');
      return;
    }

    const connect = async () => {
      if (authenticationState.user) {
        logger.warn('user is available, connecting when database is ready...');
        await database.connect(PowerSyncConnector(authenticationState, config));
        logger.warn('user is available, connected to database');
      } else if (!authenticationState.isLoading) {
        logger.info('user is null and isLoading is false, disconnecting...');
        await database.disconnect();
        logger.warn('user is null, disconnected from database');
      }
    };
    connect();
  }, [authenticationState, config, database]);

  logger.info(`PowerSyncAuthProvider rendering with user: ${authenticationState.user?.sub}`);
  return <PowerSyncContext.Provider value={database}>{children}</PowerSyncContext.Provider>;
};
