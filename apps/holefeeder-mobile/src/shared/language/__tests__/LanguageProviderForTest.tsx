import React from 'react';
import { aLanguageState } from '@/shared/language/__tests__/language-state-for-test';
import { LanguageState } from '@/shared/language/core/language-state';
import { LanguageContext } from '@/shared/language/presentation/LanguageProvider';

export const LanguageProviderForTest = ({ children, overrides }: { children: React.ReactNode; overrides?: Partial<LanguageState> }) => {
  return <LanguageContext.Provider value={aLanguageState(overrides)}>{children}</LanguageContext.Provider>;
};
