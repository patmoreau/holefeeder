import { useApiQuery } from '@/shared/api/use-api-query';
import { Tag } from '../domain/tag';

export const useTags = () => useApiQuery('/api/v2/tags/manage', (raw: Record<string, unknown>[]) => raw.map(Tag.valid));
