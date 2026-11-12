import { useApiQuery } from '@/shared/api/use-api-query';
import { Breakdown } from '../domain/breakdown';

export const useIncome = () => useApiQuery('/api/v2/insights/income', Breakdown.valid);
