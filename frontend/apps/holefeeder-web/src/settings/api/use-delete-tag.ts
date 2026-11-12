import { useApiCommand } from '@/shared/api/use-api-command';

export const useDeleteTag = () => {
  const command = useApiCommand();

  return (tag: string) => command('/api/v2/tags/delete', { tag: tag });
};
