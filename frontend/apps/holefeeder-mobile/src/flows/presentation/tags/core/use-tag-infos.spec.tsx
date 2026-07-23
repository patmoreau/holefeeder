import { renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { aTagInfo } from '@/flows/core/tags/__tests__/tag-info-for-test';
import { TagsRepositoryInMemory } from '@/flows/core/tags/__tests__/tags-repository-for-test';
import { useTagInfos } from '@/flows/presentation/tags/core/use-tag-infos';
import { RepositoryContextForTest } from '@/shared/repositories/__tests__/RepositoryContextForTest';

describe('useTagInfos', () => {
  let tagRepository: TagsRepositoryInMemory;

  const createHook = async () =>
    renderHook(() => useTagInfos(), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <RepositoryContextForTest repositories={{ tagRepository }}>{children}</RepositoryContextForTest>
      ),
    });

  beforeEach(() => {
    tagRepository = TagsRepositoryInMemory();
  });

  it('emits the tags when the repository succeeds', async () => {
    const items = [aTagInfo(), aTagInfo()];
    tagRepository.add(...items);

    const { result } = await createHook();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeSuccessWithValue(items);
  });

  it('is loading while the repository is loading', async () => {
    tagRepository.isLoading();

    const { result } = await createHook();

    expect(result.current).toBeLoading();
  });

  it('propagates a repository failure', async () => {
    tagRepository.isFailing(['boom']);

    const { result } = await createHook();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeFailureWithErrors(['boom']);
  });
});
