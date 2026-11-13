import { anId } from '@holefeeder/shared/testkit';
import { CategoriesRepositoryInMemory } from '@/flows/core/categories/__tests__/categories-repository-for-test';
import { DeactivateCategoryUseCase } from './deactivate-category-use-case';

describe('DeactivateCategoryUseCase', () => {
  let repository: CategoriesRepositoryInMemory;
  let useCase: ReturnType<typeof DeactivateCategoryUseCase>;

  beforeEach(() => {
    repository = CategoriesRepositoryInMemory();
    useCase = DeactivateCategoryUseCase(repository);
  });

  it('deactivates the category through the repository', async () => {
    const id = anId();

    const result = await useCase.execute(id);

    expect(result.isSuccess).toBe(true);
    expect(repository.deactivatedIds()).toEqual([id]);
  });

  it('returns failure when the repository fails', async () => {
    repository.isFailing(['error']);

    const result = await useCase.execute(anId());

    expect(result).toBeFailureWithErrors(['error']);
  });
});
