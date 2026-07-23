import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import type { TagInfo } from '@/flows/core/tags/tag-info';
import { WatchTagInfosUseCase } from '@/flows/core/tags/watch-tag-infos/watch-tag-infos-use-case';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export const useTagInfos = (): AsyncResult<TagInfo[]> => {
  const { tagRepository } = useRepositories();
  const [tags, setTags] = useState<AsyncResult<TagInfo[]>>(Result.loading());

  const useCase = useMemo(() => WatchTagInfosUseCase(tagRepository), [tagRepository]);

  useEffect(() => {
    const unsubscribe = useCase.watch(setTags);
    return () => unsubscribe();
  }, [useCase]);

  return tags;
};
