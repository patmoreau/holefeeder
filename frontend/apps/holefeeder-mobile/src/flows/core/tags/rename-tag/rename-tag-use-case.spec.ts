import { TagsRepositoryInMemory } from '@/flows/core/tags/__tests__/tags-repository-for-test';
import { RenameTagUseCase } from '@/flows/core/tags/rename-tag/rename-tag-use-case';
import { TagsRepositoryErrors } from '@/flows/core/tags/tags-repository';

describe('RenameTagUseCase', () => {
  let repository: TagsRepositoryInMemory;

  beforeEach(() => {
    repository = TagsRepositoryInMemory();
  });

  it('renames the tag through the repository on valid input', async () => {
    const result = await RenameTagUseCase(repository).execute('  groceries  ', '  food  ');

    expect(result).toBeSuccessWithValue(undefined);
    expect(repository.renames()).toEqual([{ oldTag: 'groceries', newTag: 'food' }]);
  });

  it('rejects an empty new tag', async () => {
    const result = await RenameTagUseCase(repository).execute('groceries', '   ');

    expect(result).toBeFailureWithErrors([TagsRepositoryErrors.invalidTagName]);
    expect(repository.renames()).toEqual([]);
  });

  it('rejects a new tag containing a comma', async () => {
    const result = await RenameTagUseCase(repository).execute('groceries', 'food,drink');

    expect(result).toBeFailureWithErrors([TagsRepositoryErrors.invalidTagName]);
    expect(repository.renames()).toEqual([]);
  });

  it('propagates a repository failure', async () => {
    repository.isFailing(['boom']);

    const result = await RenameTagUseCase(repository).execute('groceries', 'food');

    expect(result).toBeFailureWithErrors(['boom']);
  });
});
