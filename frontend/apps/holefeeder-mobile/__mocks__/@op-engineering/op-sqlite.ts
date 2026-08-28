// @powersync/react-native v2 bundles the OP-SQLite adapter, so importing it pulls in the
// native module. Jest has no native runtime, so stub the surface the adapter touches.
export const open = jest.fn();
export const getDylibPath = jest.fn(() => '');
export const moveAssetsDatabase = jest.fn();
export const isSQLCipher = jest.fn(() => false);
export const isLibsql = jest.fn(() => false);
export const isIOSEmbeeded = jest.fn(() => false);
export const IOS_LIBRARY_PATH = '';
export const IOS_DOCUMENT_PATH = '';
export const ANDROID_DATABASE_PATH = '';
export const ANDROID_FILES_PATH = '';
export const ANDROID_EXTERNAL_FILES_PATH = '';
