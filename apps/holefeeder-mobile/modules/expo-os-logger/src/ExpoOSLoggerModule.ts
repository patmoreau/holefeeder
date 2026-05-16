import { NativeModule, requireNativeModule } from 'expo-modules-core';

declare class ExpoOSLoggerModule extends NativeModule {
  log(message: string, level: string, category: string): void;
}

export default requireNativeModule<ExpoOSLoggerModule>('ExpoOSLogger');
