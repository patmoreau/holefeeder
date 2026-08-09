import 'react-native-reanimated';
import '@azure/core-asynciterator-polyfill';
import { Id, Logger } from '@holefeeder/shared/core';
import { PowerSyncDatabase } from '@powersync/react-native';
import * as Crypto from 'expo-crypto';
import { useEffect, useState } from 'react';
import { ActivityIndicator } from 'react-native';
import ErrorBoundary from 'react-native-error-boundary';
import { HolefeederContent } from '@/app/HolefeederContent';
import { HolefeederConfig } from '@/config/holefeeder-config';
import { E2eConfig } from '@/shared/auth/core/e2e-config';
import { AuthenticationProvider } from '@/shared/auth/presentation/AuthenticationProvider';
import { E2eAuthenticationProvider } from '@/shared/auth/presentation/E2eAuthenticationProvider';
import { loggerFactoryForReactNative } from '@/shared/core/logger/logger-factory-for-react-native';
import { LanguageProvider } from '@/shared/language/presentation/LanguageProvider';
import { AppStorageInMmkv } from '@/shared/persistence/app-storage-in-mmkv';
import { DatabaseFactory } from '@/shared/persistence/db';
import { PowerSyncAuthProvider } from '@/shared/persistence/presentation/PowerSyncAuthProvider';
import { RepositoryProvider } from '@/shared/repositories/presentation/RepositoryContext';
import { ThemeProvider } from '@/shared/theme/presentation/ThemeProvider';
import { CombinedInsightProvider } from '@/statistics/presentation/CombinedInsightProvider';

Id.setGenerator(Crypto.randomUUID);
Logger.setLoggerFactory(loggerFactoryForReactNative);
const logger = Logger.create('RootLayout');

// E2E builds take their session from a deep link instead of Auth0. The flag is
// stripped from production builds in app.config.ts, so this can only ever pick
// the Auth0 provider there.
const AuthProvider = E2eConfig.isEnabled() ? E2eAuthenticationProvider : AuthenticationProvider;

const RootLayout = () => {
  const [loading, setLoading] = useState(true);
  const [database, setDatabase] = useState<PowerSyncDatabase | undefined>(undefined);

  const appStorage = AppStorageInMmkv();

  const errorHandler = (error: Error, stackTrace: string) => {
    logger.error('Unhandled error', error, stackTrace);
  };

  useEffect(() => {
    let mounted = true;

    const init = async () => {
      let db: PowerSyncDatabase | undefined;
      try {
        logger.warn('Initializing database');
        // The connection lifecycle is owned solely by PowerSyncAuthProvider, which connects with the
        // authenticated connector once a user is available and disconnects when they sign out. Connecting
        // here (previously with a noop connector) started a second, credential-less connection whose
        // disconnected status leaked into the sync settings screen.
        db = await DatabaseFactory.init();
      } catch (error) {
        console.error('Database initialization error:', error);
      } finally {
        logger.warn('Initializing database done');
        if (mounted) {
          setDatabase(db);
          setLoading(false);
        }
      }
    };

    init();
    return () => {
      mounted = false;
    };
  }, []);

  const config = HolefeederConfig.parseEnv();
  if (config.isFailure) {
    logger.error('Error parsing config', config.errors);
    return <></>;
  }

  if (loading || !database) {
    return <ActivityIndicator accessibilityRole={'progressbar'} size={'large'} />;
  }

  return (
    <ErrorBoundary onError={errorHandler}>
      <LanguageProvider storage={appStorage}>
        <ThemeProvider storage={appStorage}>
          <CombinedInsightProvider storage={appStorage}>
            <AuthProvider config={config.value.authConfig}>
              <PowerSyncAuthProvider database={database} config={config.value}>
                <RepositoryProvider database={database}>
                  <HolefeederContent />
                </RepositoryProvider>
              </PowerSyncAuthProvider>
            </AuthProvider>
          </CombinedInsightProvider>
        </ThemeProvider>
      </LanguageProvider>
    </ErrorBoundary>
  );
};

export default RootLayout;
