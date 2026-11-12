import { useApiQuery } from '@/shared/api/use-api-query';
import { Cashflow } from '../domain/cashflow';

export const useCashflows = () => useApiQuery('/api/v2/cashflows/manage', (raw: Record<string, unknown>[]) => raw.map(Cashflow.valid));
