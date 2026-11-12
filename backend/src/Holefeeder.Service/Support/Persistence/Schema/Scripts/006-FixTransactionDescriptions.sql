UPDATE transactions
SET description = ''
WHERE description IS NULL;
