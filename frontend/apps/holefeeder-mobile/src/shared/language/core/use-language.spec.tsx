import { renderHook, RenderHookResult, waitFor } from '@testing-library/react-native';
import React from 'react';
import { AppStorageInMemory } from '@/shared/__tests__/app-storage-in-memory';
import { LanguageState } from '@/shared/language/core/language-state';
import { useLanguage } from '@/shared/language/core/use-language';
import { LanguageProvider } from '@/shared/language/presentation/LanguageProvider';

const LANGUAGE_KEY = 'app-settings-language';

describe('useLanguage', () => {
  const createHook = async (storage: AppStorageInMemory): Promise<RenderHookResult<LanguageState, unknown>> => {
    const hook = await renderHook(() => useLanguage(), {
      wrapper: ({ children }: { children: React.ReactNode }) => <LanguageProvider storage={storage}>{children}</LanguageProvider>,
    });
    await waitFor(() => expect(hook.result.current).toBeDefined());
    return hook;
  };

  const storageWith = (stored: string) => {
    const storage = AppStorageInMemory();
    storage.setString(LANGUAGE_KEY, stored);
    return storage;
  };

  it('defaults to English when storage is empty', async () => {
    const hook = await createHook(AppStorageInMemory());

    expect(hook.result.current.language).toBe('en');
  });

  it('restores the stored language', async () => {
    const hook = await createHook(storageWith('fr'));

    expect(hook.result.current.language).toBe('fr');
  });

  it('reads a regional stored language as its language', async () => {
    const hook = await createHook(storageWith('fr-CA'));

    expect(hook.result.current.language).toBe('fr');
  });

  it('falls back to English for an unknown stored language', async () => {
    const hook = await createHook(storageWith('de'));

    expect(hook.result.current.language).toBe('en');
  });
});
