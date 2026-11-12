import { useApiQuery } from '@/shared/api/use-api-query';
import { Breakdown } from '../domain/breakdown';

export const useSpending = () => useApiQuery('/api/v2/insights/spending', Breakdown.valid);
