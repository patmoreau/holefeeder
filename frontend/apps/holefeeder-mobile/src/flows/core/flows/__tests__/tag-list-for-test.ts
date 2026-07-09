import { TagList } from '@/flows/core/flows/tag-list';
import { aWord } from '@/shared/__tests__/string-for-test';

export const aTagList = () => TagList.valid([aWord(), aWord(), aWord()]);
