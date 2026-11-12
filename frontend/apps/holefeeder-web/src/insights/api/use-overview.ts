import { useApiQuery } from '@/shared/api/use-api-query';
import { Overview } from '../domain/overview';

export const useOverview = () => useApiQuery('/api/v2/insights/overview', Overview.valid);
