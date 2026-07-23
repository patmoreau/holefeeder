import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { AbstractPowerSyncDatabase } from '@powersync/common';
import { TagInfo } from '@/flows/core/tags/tag-info';
import { TagsRepository } from '@/flows/core/tags/tags-repository';
import { watchQuery } from '@/shared/persistence/watch-query';

type TagInfoRow = {
  tag: string;
  transactionCount: number;
  activeCashflowCount: number;
};

const TAGS_SQL = `
  WITH RECURSIVE
  tx_split(tag, remainder) AS (
    SELECT
      Ltrim(Substr(tags || ',', 1, Instr(tags || ',', ',') - 1)),
      Substr(tags || ',', Instr(tags || ',', ',') + 1)
    FROM transactions
    WHERE tags IS NOT NULL AND tags <> ''
    UNION ALL
    SELECT
      Ltrim(Substr(remainder, 1, Instr(remainder, ',') - 1)),
      Substr(remainder, Instr(remainder, ',') + 1)
    FROM tx_split
    WHERE remainder <> ''
  ),
  cf_split(tag, remainder, inactive) AS (
    SELECT
      Ltrim(Substr(tags || ',', 1, Instr(tags || ',', ',') - 1)),
      Substr(tags || ',', Instr(tags || ',', ',') + 1),
      inactive
    FROM cashflows
    WHERE tags IS NOT NULL AND tags <> ''
    UNION ALL
    SELECT
      Ltrim(Substr(remainder, 1, Instr(remainder, ',') - 1)),
      Substr(remainder, Instr(remainder, ',') + 1),
      inactive
    FROM cf_split
    WHERE remainder <> ''
  ),
  tx AS (SELECT tag FROM tx_split WHERE tag <> ''),
  cf AS (SELECT tag FROM cf_split WHERE tag <> '' AND inactive = 0),
  all_tags AS (SELECT tag FROM tx UNION SELECT tag FROM cf)
  SELECT
    a.tag                                                  AS tag,
    (SELECT COUNT(*) FROM tx WHERE tx.tag = a.tag)         AS transactionCount,
    (SELECT COUNT(*) FROM cf WHERE cf.tag = a.tag)         AS activeCashflowCount
  FROM all_tags a
  ORDER BY a.tag ASC
`;

const TAGGED_TABLES = ['transactions', 'cashflows'] as const;

const renameTagInList = (tags: string, oldTag: string, newTag: string): string => {
  const renamed = tags.split(',').map((tag) => (tag === oldTag ? newTag : tag));
  return renamed.filter((tag, index) => renamed.indexOf(tag) === index).join(',');
};

const removeTagFromList = (tags: string, tag: string): string =>
  tags
    .split(',')
    .filter((current) => current !== tag)
    .join(',');

const selectRowsWithTag = (tx: { getAll: <T>(sql: string, params: unknown[]) => Promise<T[]> }, table: string, tag: string) =>
  tx.getAll<{ id: string; tags: string }>(
    `SELECT id, tags FROM ${table}
     WHERE tags IS NOT NULL AND tags <> ''
       AND Instr(',' || tags || ',', ',' || ? || ',') > 0`,
    [tag]
  );

export const TagsRepositoryInPowersync = (db: AbstractPowerSyncDatabase): TagsRepository => {
  const watch = (onDataChange: (result: AsyncResult<TagInfo[]>) => void) => {
    return watchQuery<TagInfoRow, TagInfo>(
      db,
      TAGS_SQL,
      [],
      (row) => ({ tag: row.tag, transactionCount: row.transactionCount, activeCashflowCount: row.activeCashflowCount }),
      onDataChange,
      'watchTags'
    );
  };

  const rename = async (oldTag: string, newTag: string): Promise<Result<void>> => {
    try {
      await db.writeTransaction(async (tx) => {
        for (const table of TAGGED_TABLES) {
          const rows = await selectRowsWithTag(tx, table, oldTag);
          for (const row of rows) {
            await tx.execute(`UPDATE ${table} SET tags = ? WHERE id = ?`, [renameTagInList(row.tags, oldTag, newTag), row.id]);
          }
        }
      });
      return Result.success();
    } catch (error) {
      return Result.failure([(error as Error)?.message ?? String(error)]);
    }
  };

  const remove = async (tag: string): Promise<Result<void>> => {
    try {
      await db.writeTransaction(async (tx) => {
        for (const table of TAGGED_TABLES) {
          const rows = await selectRowsWithTag(tx, table, tag);
          for (const row of rows) {
            await tx.execute(`UPDATE ${table} SET tags = ? WHERE id = ?`, [removeTagFromList(row.tags, tag), row.id]);
          }
        }
      });
      return Result.success();
    } catch (error) {
      return Result.failure([(error as Error)?.message ?? String(error)]);
    }
  };

  return { watch: watch, rename: rename, remove: remove };
};
