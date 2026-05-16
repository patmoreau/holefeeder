import { faker } from '@faker-js/faker';
import { Id } from '@/shared/core/id';

export const aColor = () => faker.color.rgb();

export const anId = () => Id.valid(faker.string.uuid());

export const aString = () => faker.lorem.words(3);

export const aWord = () => faker.lorem.word();

export const aUrl = () => faker.internet.url();

export const aToken = () => faker.internet.jwt();
