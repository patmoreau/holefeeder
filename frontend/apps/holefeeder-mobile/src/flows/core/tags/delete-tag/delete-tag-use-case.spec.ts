import { TagsRepositoryInMemory } from '@/flows/core/tags/__tests__/tags-repository-for-test';
import { DeleteTagUseCase } from '@/flows/core/tags/delete-tag/delete-tag-use-case';
import { TagsRepositoryErrors } from '@/flows/core/tags/tags-repository';

describe('DeleteTagUseCase', () => {
  let repository: TagsRepositoryInMemory;

  beforeEach(() => {
    repository = TagsRepositoryInMemory();
  });

  it('removes the tag through the repository', async () => {
    const result = await DeleteTagUseCase(repository).execute('  groceries  ');

    expect(result).toBeSuccessWithValue(undefined);
    expect(repository.removed()).toEqual(['groceries']);
  });

  it('rejects an empty tag', async () => {
    const result = await DeleteTagUseCase(repository).execute('   ');

    expect(result).toBeFailureWithErrors([TagsRepositoryErrors.invalidTagName]);
    expect(repository.removed()).toEqual([]);
  });

  it('propagates a repository failure', async () => {
    repository.isFailing(['boom']);

    const result = await DeleteTagUseCase(repository).execute('groceries');

    expect(result).toBeFailureWithErrors(['boom']);
  });
});
