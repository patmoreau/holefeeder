import { randomUUID } from 'crypto';
import { Id } from '../../id';

const words = ['apple', 'blue', 'cloud', 'delta', 'echo', 'fox', 'golf', 'hotel', 'india', 'jazz'];

const pick = (arr: string[]) => arr[Math.floor(Math.random() * arr.length)];

export const aColor = () => `#${Math.floor(Math.random() * 0xffffff).toString(16).padStart(6, '0')}`;

export const anId = () => Id.valid(randomUUID());

export const aString = () => `${pick(words)} ${pick(words)} ${pick(words)}`;

export const aWord = () => pick(words);

export const aUrl = () => `https://${pick(words)}.example.com`;

export const aToken = () =>
  `eyJhbGciOiJIUzI1NiJ9.${Buffer.from(JSON.stringify({ sub: randomUUID() })).toString('base64url')}.sig`;
