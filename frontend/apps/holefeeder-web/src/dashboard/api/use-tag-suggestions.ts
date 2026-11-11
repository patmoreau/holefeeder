import { useApiQuery } from '@/shared/api/use-api-query';

export const useTagSuggestions = (categoryId: string) =>
  useApiQuery(`/api/v2/transactions/tag-suggestions?categoryId=${encodeURIComponent(categoryId)}`, (tags: string[]) => tags);
