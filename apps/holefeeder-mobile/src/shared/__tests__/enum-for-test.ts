import { faker } from '@faker-js/faker';
import { DateIntervalType, DateIntervalTypes } from '@holefeeder/core';
import { AccountType, AccountTypes } from '@/accounts/core/account-type';
import { CategoryType, CategoryTypes } from '@/flows/core/categories/category-type';

export const anAccountType = (): AccountType => faker.helpers.arrayElement(Object.values(AccountTypes)) as AccountType;

export const aCategoryType = (): CategoryType => faker.helpers.arrayElement(Object.values(CategoryTypes)) as CategoryType;

export const aDateIntervalType = (): DateIntervalType => faker.helpers.arrayElement(Object.values(DateIntervalTypes)) as DateIntervalType;
