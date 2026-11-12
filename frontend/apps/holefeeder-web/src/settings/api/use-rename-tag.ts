import { useApiCommand } from '@/shared/api/use-api-command';

export const useRenameTag = () => {
  const command = useApiCommand();

  return (tag: string, newTag: string) => command('/api/v2/tags/rename', { tag: tag, newTag: newTag });
};
