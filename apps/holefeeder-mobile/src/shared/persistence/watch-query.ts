import { AbstractPowerSyncDatabase, QueryParam } from '@powersync/common';
import { Logger } from '@/shared/core/logger/logger';
import { type AsyncResult, Result } from '@/shared/core/result';

const logger = Logger.create('watch-query');

export const watchQuery = <TRow, T>(
  db: AbstractPowerSyncDatabase,
  sql: string,
  parameters: readonly Readonly<QueryParam>[],
  mapRow: (row: TRow) => T,
  onDataChange: (result: AsyncResult<T[]>) => void,
  name: string = 'unknown'
): (() => void) => {
  logger.debug(`Registering watch: "${name}"`);

  const watcher = db.query<TRow>({ sql, parameters }).watch();

  return watcher.registerListener({
    onData: (data) => {
      logger.debug(`Watch data received: "${name}" (${data?.length ?? 0} rows)`);
      onDataChange(!data || data.length === 0 ? Result.success([]) : Result.success(data.map(mapRow)));
    },
    onError: (error) => {
      logger.error(`Error in watch: "${name}"`, error);
      onDataChange(Result.failure([error.message]));
    },
  });
};

export const WatchQueryErrors = {
  rowNotFound: 'row-not-found',
};

export const watchSingle = <TRow, T>(
  db: AbstractPowerSyncDatabase,
  sql: string,
  parameters: readonly Readonly<QueryParam>[],
  onRow: (row: TRow) => T,
  onDataChange: (result: AsyncResult<T>) => void,
  onEmpty: () => Result<T> = () => Result.failure([WatchQueryErrors.rowNotFound]),
  name: string = 'unknown'
): (() => void) => {
  logger.debug(`Registering watch: "${name}"`);

  const watcher = db.query<TRow>({ sql, parameters }).watch();

  return watcher.registerListener({
    onData: (data) => {
      logger.debug(`Watch data received: "${name}" (${data?.length ?? 0} rows)`);
      onDataChange(!data || data.length === 0 ? onEmpty() : Result.success(onRow(data[0])));
    },
    onError: (error) => onDataChange(Result.failure([error.message])),
  });
};
