import { renderHook } from '@testing-library/react-native';
import { useSyncStatus } from '@/settings/presentation/core/use-sync-status';

const mockUseStatus = jest.fn();
jest.mock('@powersync/react', () => ({
  useStatus: () => mockUseStatus(),
}));

describe('useSyncStatus', () => {
  beforeEach(() => {
    mockUseStatus.mockReset();
  });

  it('maps a fully populated status', async () => {
    const lastSyncedAt = new Date('2026-07-18T10:00:00Z');
    mockUseStatus.mockReturnValue({
      connected: true,
      lastSyncedAt,
      dataFlowStatus: { downloading: true, uploading: false },
    });

    const { result } = await renderHook(() => useSyncStatus());

    expect(result.current).toEqual({
      connected: true,
      lastSyncedAt,
      dataFlowStatus: { downloading: true, uploading: false },
    });
  });

  it('applies defaults when status fields are absent', async () => {
    mockUseStatus.mockReturnValue({});

    const { result } = await renderHook(() => useSyncStatus());

    expect(result.current).toEqual({
      connected: false,
      lastSyncedAt: null,
      dataFlowStatus: { downloading: false, uploading: false },
    });
  });

  it('defaults dataFlow fields when the dataFlowStatus object omits them', async () => {
    mockUseStatus.mockReturnValue({ connected: true, dataFlowStatus: {} });

    const { result } = await renderHook(() => useSyncStatus());

    expect(result.current.dataFlowStatus).toEqual({ downloading: false, uploading: false });
  });
});
