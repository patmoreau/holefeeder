import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { type AsyncResult, Result } from '@/shared/core/result';

export type WatchPageFn<T> = (onData: (items: T[]) => void, limit: number, offset: number) => () => void;
export type WatchCountFn = (onCount: (count: number) => void) => () => void;

export type UsePagedWatchOptions = {
  pageSize?: number;
  maxPages?: number;
};

export type UsePagedWatchResult<T> = {
  data: AsyncResult<T[]>;
  totalCount: number;
  loadNext: () => void;
  loadPrevious: () => void;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  loading: boolean;
};

const DEFAULT_PAGE_SIZE = 100;
const DEFAULT_MAX_PAGES = 3;

export const usePagedWatch = <T>(
  watchFn: WatchPageFn<T>,
  watchCountFn: WatchCountFn,
  options?: UsePagedWatchOptions
): UsePagedWatchResult<T> => {
  const { pageSize = DEFAULT_PAGE_SIZE, maxPages = DEFAULT_MAX_PAGES } = options ?? {};

  const [activePages, setActivePages] = useState<number[]>([0]);
  const [pageData, setPageData] = useState<Record<number, T[]>>({});
  const [totalCount, setTotalCount] = useState<number>(0);
  const [loadingState, setLoadingState] = useState(false);

  // Use a ref for sync access during rapid scroll events
  const isLoadingRef = useRef(false);
  const subsRef = useRef<Map<number, () => void>>(new Map());

  const purgeObsoletePages = useCallback(() => {
    let changed = false;
    const nextData = { ...pageData };

    subsRef.current.forEach((unsub, idx) => {
      if (!activePages.includes(idx)) {
        unsub();
        subsRef.current.delete(idx);
        delete nextData[idx];
        changed = true;
      }
    });

    if (changed) setPageData(nextData);
  }, [activePages, pageData]);

  const subscribeToMissingPages = useCallback(() => {
    activePages.forEach((idx) => {
      if (!subsRef.current.has(idx)) {
        const unsub = watchFn(
          (items) => {
            setPageData((prev) => ({ ...prev, [idx]: items }));
            isLoadingRef.current = false;
            setLoadingState(false);
          },
          pageSize,
          idx * pageSize
        );
        subsRef.current.set(idx, unsub);
      }
    });
  }, [activePages, watchFn, pageSize]);

  // Handle subscriptions and purges
  useEffect(() => {
    purgeObsoletePages();
    subscribeToMissingPages();
  }, [activePages, purgeObsoletePages, subscribeToMissingPages]);

  // Strict cleanup on unmount
  useEffect(() => {
    const currentSubs = subsRef.current;
    return () => {
      currentSubs.forEach((unsub) => unsub());
      currentSubs.clear();
    };
  }, []);

  useEffect(() => {
    return watchCountFn(setTotalCount);
  }, [watchCountFn]);

  const sortedIndices = useMemo(() => [...activePages].sort((a, b) => a - b), [activePages]);
  const minPage = sortedIndices[0] ?? 0;
  const maxPage = sortedIndices[sortedIndices.length - 1] ?? 0;

  const data = useMemo(() => {
    // If we have at least one page of data, we consider it a success
    if (Object.keys(pageData).length === 0) return Result.loading();
    const flat = sortedIndices.flatMap((i) => pageData[i] ?? []);
    return Result.success(flat);
  }, [pageData, sortedIndices]);

  const hasNextPage = (maxPage + 1) * pageSize < totalCount;
  const hasPreviousPage = minPage > 0;

  const loadNext = useCallback(() => {
    if (isLoadingRef.current || !hasNextPage) return;

    isLoadingRef.current = true;
    setLoadingState(true);

    setActivePages((prev) => {
      const next = [...prev, maxPage + 1];
      return next.length > maxPages ? next.slice(1) : next;
    });
  }, [hasNextPage, maxPage, maxPages]);

  const loadPrevious = useCallback(() => {
    if (isLoadingRef.current || !hasPreviousPage) return;

    isLoadingRef.current = true;
    setLoadingState(true);

    setActivePages((prev) => {
      const next = [minPage - 1, ...prev];
      return next.length > maxPages ? next.slice(0, -1) : next;
    });
  }, [hasPreviousPage, minPage, maxPages]);

  return {
    data,
    totalCount,
    loadNext,
    loadPrevious,
    hasNextPage,
    hasPreviousPage,
    loading: loadingState,
  };
};
