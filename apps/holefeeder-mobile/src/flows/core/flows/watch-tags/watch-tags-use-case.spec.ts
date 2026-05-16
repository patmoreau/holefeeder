import { waitFor } from '@testing-library/react-native';
import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { aTag } from '@/flows/core/flows/__tests__/tag-for-test';
import { Tag } from '@/flows/core/flows/tag';
import { type AsyncResult } from '@/shared/core/result';
import { WatchTagsUseCase } from './watch-tags-use-case';

describe('WatchTagsUseCase', () => {
  let repository: FlowsRepositoryInMemory;
  let useCase: ReturnType<typeof WatchTagsUseCase>;

  beforeEach(() => {
    repository = FlowsRepositoryInMemory();
    useCase = WatchTagsUseCase(repository);
  });

  it('should return tags when repository succeeds', async () => {
    const tag = aTag();
    repository.addTags(tag);

    let result: AsyncResult<Tag[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeSuccessWithValue([tag]);

    unsubscribe();
  });

  it('should return failure when repository fails', async () => {
    repository.isFailing(['error']);

    let result: AsyncResult<Tag[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeFailureWithErrors(['error']);

    unsubscribe();
  });

  it('should return loading when repository is loading', async () => {
    repository.isLoading();

    let result: AsyncResult<Tag[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeLoading();

    unsubscribe();
  });
});
