import { aWord } from '@holefeeder/shared/testkit';
import { TagList } from '@/flows/core/flows/tag-list';

export const aTagList = () => TagList.valid([aWord(), aWord(), aWord()]);
