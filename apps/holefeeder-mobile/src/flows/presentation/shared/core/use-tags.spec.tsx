import { act, renderHook, waitFor } from '@testing-library/react-native';
import { aTag } from '@/flows/core/flows/__tests__/tag-for-test';
import { aTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { Tag } from '@/flows/core/flows/tag';
import { TagList } from '@/flows/core/flows/tag-list';
import { Id } from '@/shared/core/id';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { PowerSyncProviderForTest } from '@/shared/persistence/__tests__/PowerSyncProviderForTest';
import { useTags } from './use-tags';

describe('useTags', () => {
  let db: DatabaseForTest;
  const categoryId = Id.newId();
  const firstTransaction = aTransaction({ categoryId, tags: TagList.valid(['groceries', 'food']) });
  const secondTransaction = aTransaction({ categoryId, tags: TagList.valid(['groceries', 'shopping']) });
  const thirdTransaction = aTransaction({ categoryId, tags: TagList.valid(['food']) });
  const validTags: Tag[] = [
    aTag({ tag: 'food', count: 2, categoryId }),
    aTag({ tag: 'groceries', count: 2, categoryId }),
    aTag({ tag: 'shopping', count: 1, categoryId }),
  ];

  const createHook = async () =>
    await waitFor(() =>
      renderHook(() => useTags(), {
        wrapper: ({ children }: { children: React.ReactNode }) => <PowerSyncProviderForTest database={db}>{children}</PowerSyncProviderForTest>,
      })
    );

  beforeEach(async () => {
    db = await setupDatabaseForTest();

    await firstTransaction.store(db);
    await secondTransaction.store(db);
    await thirdTransaction.store(db);
  });

  afterEach(async () => {
    await act(async () => {
      await db.cleanupTestDb();
    });
  });

  it('should fetch tags from PowerSync database', async () => {
    const { result } = await createHook();

    expect(result.current).toBeLoading();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeSuccessWithValue(validTags);
  });
});
