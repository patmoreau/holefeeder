import { CategoriesRepositoryInMemory } from '@/flows/core/categories/__tests__/categories-repository-for-test';
import { aCreateCategoryCommand } from '@/flows/core/categories/create/__tests__/create-category-command-for-test';
import { CreateCategoryUseCase } from '@/flows/core/categories/create/create-category-use-case';

describe('CreateCategoryUseCase', () => {
  let repository: CategoriesRepositoryInMemory;
  let useCase: ReturnType<typeof CreateCategoryUseCase>;

  beforeEach(() => {
    repository = CategoriesRepositoryInMemory();
    useCase = CreateCategoryUseCase(repository);
  });

  it('should create category with valid data', async () => {
    const result = await useCase.execute(aCreateCategoryCommand());

    expect(result).toBeSuccessWithValue(expect.anything());
  });
});
