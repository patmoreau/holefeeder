import { NativeModule, requireNativeModule } from 'expo';

declare class ExpoEndEditingModule extends NativeModule {
  endEditing(): Promise<void>;
}

export default requireNativeModule<ExpoEndEditingModule>('ExpoEndEditing');
