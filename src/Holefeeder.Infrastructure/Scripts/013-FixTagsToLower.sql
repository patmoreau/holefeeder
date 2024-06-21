UPDATE transactions
SET tags = LOWER(tags);

UPDATE cashflows
SET tags = LOWER(tags);
