import { requireOptionalNativeModule } from 'expo';

declare class CollapsingHeaderStoreModule {
  discardStore(id: string): void;
}

// Optional so the modifiers stay importable off a device, where no native module is registered.
const nativeModule = requireOptionalNativeModule<CollapsingHeaderStoreModule>('CollapsingHeaderModule');

/**
 * Releases the native store the collapsing header registered under `id`. The registry that holds
 * it is a process-wide singleton, so a header that goes away without calling this leaves an entry
 * behind for the life of the app.
 *
 * The function is called optionally as well as the module: a reloaded JavaScript bundle can run
 * against a native binary built before this existed, and a leaked store is a far smaller problem
 * than every screen with a header throwing as it unmounts.
 */
export const discardCollapsingHeaderStore = (id: string) => nativeModule?.discardStore?.(id);
