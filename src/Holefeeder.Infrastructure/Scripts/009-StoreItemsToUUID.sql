-- Drop the primary key constraint
ALTER TABLE store_items DROP PRIMARY KEY;

-- Alter the columns from BINARY(16) to UUID
ALTER TABLE store_items
    MODIFY id UUID NOT NULL,
    MODIFY user_id UUID NOT NULL;

-- Re-add the primary key constraint
ALTER TABLE store_items ADD PRIMARY KEY (id);

-- Drop the existing unique index
DROP INDEX user_id_code_idx ON store_items;

-- Re-create the unique index on (user_id, code)
CREATE UNIQUE INDEX user_id_code_idx ON store_items (user_id, code);
