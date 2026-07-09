import { Result } from './result';
import { Validate, Validator } from './validate';

export type Id = string & { readonly _id: unique symbol };

export const IdErrors = {
  invalid: 'id-invalid',
};

const UUID_PATTERN = /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/;
const isValidPattern = Validator.pattern<Id>({ pattern: UUID_PATTERN });

// Default uses the Web Crypto API available in modern browsers and Node 19+.
// Call Id.setGenerator() in platform-specific bootstrap when needed (e.g. expo-crypto on React Native).
let _uuidGenerator: () => string = () => {
  if (typeof crypto !== 'undefined' && typeof crypto.randomUUID === 'function') {
    return crypto.randomUUID();
  }
  throw new Error('No UUID generator available. Call Id.setGenerator() before use.');
};

const setGenerator = (fn: () => string): void => {
  _uuidGenerator = fn;
};

const newId = (): Id => _uuidGenerator() as Id;

const create = (value: unknown): Result<Id> => Validate.validate<Id>(isValidPattern, value, [IdErrors.invalid]);

const valid = (value: unknown): Id => value as Id;

export const Id = { newId, create, valid, setGenerator } as const;
