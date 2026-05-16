import { act, renderHook, waitFor } from '@testing-library/react-native';
import { anAccount } from '@/accounts/core/__tests__/account-for-test';
import { aCategory } from '@/flows/core/categories/__tests__/category-for-test';
import { aCashflow } from '@/flows/core/flows/__tests__/cashflow-for-test';
import { aTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { useSyncInfo } from '@/settings/presentation/core/use-sync-info';
import { useSyncStatus } from '@/settings/presentation/core/use-sync-status';
import { aStoreItem } from '@/shared/__tests__/store-item-for-test';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { PowerSyncProviderForTest } from '@/shared/persistence/__tests__/PowerSyncProviderForTest';

jest.mock('@/settings/presentation/core/use-sync-status');

describe('useSyncInfo', () => {
  let db: DatabaseForTest;

  const mockSyncStatus = {
    connected: true,
    lastSyncedAt: new Date('2023-01-01T12:00:00Z'),
    dataFlowStatus: {
      downloading: false,
      uploading: false,
    },
  };

  const createHook = async () =>
    await waitFor(() =>
      renderHook(() => useSyncInfo(), {
        wrapper: ({ children }: { children: React.ReactNode }) => <PowerSyncProviderForTest database={db}>{children}</PowerSyncProviderForTest>,
      })
    );

  beforeEach(async () => {
    db = await setupDatabaseForTest();
    await db.execute('CREATE TABLE ps_crud (id INTEGER PRIMARY KEY);');
    await anAccount().store(db);
    await aCategory().store(db);
    await aCashflow().store(db);
    await aStoreItem().store(db);
    await aTransaction().store(db);
    await db.execute('INSERT INTO ps_crud (id) VALUES (1);');

    (useSyncStatus as jest.Mock).mockReturnValue(mockSyncStatus);
  });

  afterEach(async () => {
    await act(async () => {
      await db.cleanupTestDb();
    });
  });

  it('should fetch sync info from PowerSync database', async () => {
    const { result } = await createHook();

    expect(result.current).toBeLoading();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeSuccessWithValue({
      connected: true,
      lastSyncedAt: new Date('2023-01-01T12:00:00Z'),
      dataFlowStatus: {
        downloading: false,
        uploading: false,
      },
      dataMetrics: {
        transactions: 1,
        cashflows: 1,
        categories: 1,
        accounts: 1,
        outstandingTransactions: 1,
        storeItems: 1,
      },
    });
  });
});
