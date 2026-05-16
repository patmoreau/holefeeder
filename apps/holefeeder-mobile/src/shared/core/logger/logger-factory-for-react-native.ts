import { Platform } from 'react-native';
import { consoleTransport, type defLvlType, logger as RNLogger, type LoggerInstance } from 'react-native-logs';
import ExpoOSLogger from '@/modules/expo-os-logger';
import { LoggerFactory } from '@/shared/core/logger/logger';

const nativeTransport = (props: any) => {
  ExpoOSLogger.log(props.msg, props.level.text, props.extension ?? '');
};

const loggingEnabled = __DEV__ || process.env.EXPO_PUBLIC_FORCE_LOGS === 'true';

type BaseLogger = ReturnType<typeof RNLogger.createLogger>;

let _instance: BaseLogger | null = null;

const getInstance = (): BaseLogger => {
  if (_instance === null) {
    _instance = RNLogger.createLogger({
      severity: 'debug',
      transport: Platform.OS === 'ios' && !__DEV__ ? nativeTransport : consoleTransport,
      transportOptions: {
        colors: {
          debug: 'white',
          info: 'blueBright',
          warn: 'yellowBright',
          error: 'redBright',
        },
      },
      async: true,
      dateFormat: 'time',
      printDate: true,
      printLevel: true,
      enabled: loggingEnabled,
    });
  }
  return _instance;
};

export const loggerFactoryForReactNative: LoggerFactory = (moduleName: string) =>
  getInstance().extend(moduleName) as LoggerInstance<defLvlType>;
