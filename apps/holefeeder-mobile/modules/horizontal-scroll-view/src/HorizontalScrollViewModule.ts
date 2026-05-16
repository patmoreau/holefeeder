import { NativeModule, requireNativeModule } from 'expo';
import { HorizontalScrollViewModuleEvents } from './HorizontalScrollView.types';

declare class HorizontalScrollViewModule extends NativeModule<HorizontalScrollViewModuleEvents> {
  PI: number;
  hello(): string;
  setValueAsync(value: string): Promise<void>;
}

// This call loads the native module object from the JSI.
export default requireNativeModule<HorizontalScrollViewModule>('HorizontalScrollView');
