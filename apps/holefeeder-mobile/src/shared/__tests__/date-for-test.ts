import { faker } from '@faker-js/faker';
import { DateOnly } from '@/shared/core/date-only';

export const aPastDate = () => DateOnly.valid(faker.date.past().toISOString().split('T')[0]);

export const aRecentDate = () => DateOnly.valid(faker.date.recent().toISOString().split('T')[0]);

export const aFutureDate = () => DateOnly.valid(faker.date.future().toISOString().split('T')[0]);

export const aTimestamp = () => faker.date.future().valueOf();
