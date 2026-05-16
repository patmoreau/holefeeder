import { useContext } from 'react';
import { RepositoriesState } from '@/shared/repositories/core/repositories-state';
import { RepositoryContext } from '@/shared/repositories/presentation/RepositoryContext';

export const useRepositories = (): RepositoriesState => {
  const context = useContext(RepositoryContext);
  if (!context) {
    throw new Error('useRepositories must be used within a RepositoryProvider');
  }
  return context;
};
