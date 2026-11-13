import { faker } from '@faker-js/faker';
import { CategoryType, CategoryTypes, AccountType, AccountTypes, DateIntervalType, DateIntervalTypes } from '@holefeeder/shared/core';

export const anAccountType = (): AccountType => faker.helpers.arrayElement(Object.values(AccountTypes)) as AccountType;

export const aCategoryType = (): CategoryType => faker.helpers.arrayElement(Object.values(CategoryTypes)) as CategoryType;

export const aDateIntervalType = (): DateIntervalType => faker.helpers.arrayElement(Object.values(DateIntervalTypes)) as DateIntervalType;
