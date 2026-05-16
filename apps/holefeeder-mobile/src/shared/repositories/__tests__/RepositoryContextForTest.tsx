import React from 'react';
import { aRepositoriesState } from '@/shared/repositories/__tests__/repositories-state-for-test';
import { RepositoriesState } from '@/shared/repositories/core/repositories-state';
import { RepositoryContext } from '@/shared/repositories/presentation/RepositoryContext';

export const RepositoryContextForTest = ({
  children,
  repositories,
}: {
  children: React.ReactNode;
  repositories?: Partial<RepositoriesState>;
}) => {
  return <RepositoryContext.Provider value={aRepositoriesState(repositories)}>{children}</RepositoryContext.Provider>;
};
