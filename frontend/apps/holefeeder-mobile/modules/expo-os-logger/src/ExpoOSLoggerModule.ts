import { NativeModule, requireNativeModule } from 'expo';

declare class ExpoOSLoggerModule extends NativeModule {
  log(message: string, level: string, category: string): void;
}

export default requireNativeModule<ExpoOSLoggerModule>('ExpoOSLogger');
