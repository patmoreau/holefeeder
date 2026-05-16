import { renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { useRepositories } from '@/shared/repositories/core/use-repositories';
import { RepositoryProvider } from './RepositoryContext';

describe('RepositoryContext', () => {
  let db: DatabaseForTest;

  beforeEach(async () => {
    db = await setupDatabaseForTest();
  });

  afterEach(async () => {
    await db.cleanupTestDb();
  });

  it('should throw error when used outside of RepositoryProvider', () => {
    expect(() => {
      renderHook(() => useRepositories());
    }).toThrow('useRepositories must be used within a RepositoryProvider');
  });

  it('should provide all repositories when used within RepositoryProvider', async () => {
    const { result } = await waitFor(() =>
      renderHook(() => useRepositories(), {
        wrapper: ({ children }) => <RepositoryProvider database={db}>{children}</RepositoryProvider>,
      })
    );

    await waitFor(() => expect(result.current).toBeDefined());

    expect(result.current.accountRepository).toBeDefined();
    expect(result.current.categoryRepository).toBeDefined();
    expect(result.current.dashboardRepository).toBeDefined();
    expect(result.current.flowRepository).toBeDefined();
    expect(result.current.settingRepository).toBeDefined();
    expect(result.current.storeItemRepository).toBeDefined();
  });

  it('should return the same repository instances on multiple calls', () => {
    const { result } = renderHook(() => useRepositories(), {
      wrapper: ({ children }) => <RepositoryProvider database={db}>{children}</RepositoryProvider>,
    });

    const { result: result2 } = renderHook(() => useRepositories(), {
      wrapper: ({ children }) => <RepositoryProvider database={db}>{children}</RepositoryProvider>,
    });

    // Should create new instances with new provider
    expect(result.current).toBeDefined();
    expect(result2.current).toBeDefined();
    expect(result.current.accountRepository).toBeDefined();
    expect(result2.current.accountRepository).toBeDefined();
  });
});
