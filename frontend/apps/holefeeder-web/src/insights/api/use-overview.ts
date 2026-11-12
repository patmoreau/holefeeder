import { useApiQuery } from '@/shared/api/use-api-query';
import { Overview } from '../domain/overview';

export const useOverview = (periods: number) => useApiQuery(`/api/v2/insights/overview?periods=${periods}`, Overview.valid);
