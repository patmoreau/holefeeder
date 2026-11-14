import type { en } from '@holefeeder/shared/core';
import 'react-i18next';

// Tell react-i18next the shape of our resources
declare module 'react-i18next' {
  interface CustomTypeOptions {
    // This makes t('…') keys type-safe using the English schema
    resources: {
      translation: typeof en;
    };
  }
}
