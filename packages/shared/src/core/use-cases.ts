import { type AsyncResult, Result } from './result';

export type Command<O, I> = {
  execute(input: I): Promise<Result<O>>;
};

export type Query<I, O> = I extends void
  ? {
      query: (onDataChange: (result: AsyncResult<O>) => void) => () => void;
    }
  : {
      query: (input: I, onDataChange: (result: AsyncResult<O>) => void) => () => void;
    };
