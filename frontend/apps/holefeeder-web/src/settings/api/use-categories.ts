import { useApiQuery } from '@/shared/api/use-api-query';
import { Category } from '../domain/category';

export const useCategories = () => useApiQuery('/api/v2/categories/manage', (raw: Record<string, unknown>[]) => raw.map(Category.valid));
