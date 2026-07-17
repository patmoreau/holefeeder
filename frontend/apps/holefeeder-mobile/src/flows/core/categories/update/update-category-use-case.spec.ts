import { CategoriesRepositoryInMemory } from '@/flows/core/categories/__tests__/categories-repository-for-test';
import { anUpdateCategoryCommand } from '@/flows/core/categories/update/__tests__/update-category-command-for-test';
import { UpdateCategoryUseCase } from '@/flows/core/categories/update/update-category-use-case';

describe('UpdateCategoryUseCase', () => {
  let repository: CategoriesRepositoryInMemory;
  let useCase: ReturnType<typeof UpdateCategoryUseCase>;

  beforeEach(() => {
    repository = CategoriesRepositoryInMemory();
    useCase = UpdateCategoryUseCase(repository);
  });

  it('should update category and return its id', async () => {
    const command = anUpdateCategoryCommand();

    const result = await useCase.execute(command);

    expect(result).toBeSuccessWithValue(command.id);
  });
});
