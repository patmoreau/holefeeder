// Names live in the translations, not here: whatever the backend or this list wrote
// would be frozen text in a database row, and a French user would keep English
// category names forever. The client knows the locale, so it picks the words.
export const SuggestedCategoryKeys = [
  'groceries',
  'restaurants',
  'transportation',
  'housing',
  'utilities',
  'health',
  'entertainment',
  'shopping',
] as const;

export type SuggestedCategoryKey = (typeof SuggestedCategoryKeys)[number];

export const SuggestedCategoryColors: Record<SuggestedCategoryKey, string> = {
  groceries: '#4CAF50',
  restaurants: '#FF7043',
  transportation: '#42A5F5',
  housing: '#8D6E63',
  utilities: '#FFCA28',
  health: '#EF5350',
  entertainment: '#AB47BC',
  shopping: '#EC407A',
};
