-- Drop foreign key constraints in cashflows and transactions tables
ALTER TABLE cashflows DROP FOREIGN KEY fk_cashflow_account;
ALTER TABLE cashflows DROP FOREIGN KEY fk_cashflow_category;
ALTER TABLE transactions DROP FOREIGN KEY fk_transaction_account;
ALTER TABLE transactions DROP FOREIGN KEY fk_transaction_category;
ALTER TABLE transactions DROP FOREIGN KEY fk_transaction_cashflow;

-- Drop primary key constraints
ALTER TABLE accounts DROP PRIMARY KEY;
ALTER TABLE categories DROP PRIMARY KEY;
ALTER TABLE cashflows DROP PRIMARY KEY;
ALTER TABLE transactions DROP PRIMARY KEY;

-- Drop unique indexes
DROP INDEX user_id_name_idx ON accounts;
DROP INDEX user_id_name_idx ON categories;

-- Alter columns from BINARY(16) to UUID
ALTER TABLE accounts
    MODIFY id UUID NOT NULL,
    MODIFY user_id UUID NOT NULL;

ALTER TABLE categories
    MODIFY id UUID NOT NULL,
    MODIFY user_id UUID NOT NULL;

ALTER TABLE cashflows
    MODIFY id UUID NOT NULL,
    MODIFY account_id UUID NOT NULL,
    MODIFY category_id UUID NOT NULL,
    MODIFY user_id UUID NOT NULL;

ALTER TABLE transactions
    MODIFY id UUID NOT NULL,
    MODIFY account_id UUID NOT NULL,
    MODIFY category_id UUID NOT NULL,
    MODIFY cashflow_id UUID,
    MODIFY user_id UUID NOT NULL;

-- Re-add primary key constraints
ALTER TABLE accounts ADD PRIMARY KEY (id);
ALTER TABLE categories ADD PRIMARY KEY (id);
ALTER TABLE cashflows ADD PRIMARY KEY (id);
ALTER TABLE transactions ADD PRIMARY KEY (id);

-- Re-create unique indexes
CREATE UNIQUE INDEX user_id_name_idx ON accounts (user_id, name);
CREATE UNIQUE INDEX user_id_name_idx ON categories (user_id, name);

-- Re-add foreign key constraints in cashflows and transactions tables
ALTER TABLE cashflows
    ADD CONSTRAINT fk_cashflow_account FOREIGN KEY (account_id) REFERENCES accounts (id)
        ON DELETE RESTRICT
        ON UPDATE RESTRICT,
    ADD CONSTRAINT fk_cashflow_category FOREIGN KEY (category_id) REFERENCES categories (id)
        ON DELETE RESTRICT
        ON UPDATE RESTRICT;

ALTER TABLE transactions
    ADD CONSTRAINT fk_transaction_account FOREIGN KEY (account_id) REFERENCES accounts (id)
        ON DELETE RESTRICT
        ON UPDATE RESTRICT,
    ADD CONSTRAINT fk_transaction_category FOREIGN KEY (category_id) REFERENCES categories (id)
        ON DELETE RESTRICT
        ON UPDATE RESTRICT,
    ADD CONSTRAINT fk_transaction_cashflow FOREIGN KEY (cashflow_id) REFERENCES cashflows (id)
        ON DELETE RESTRICT
        ON UPDATE RESTRICT;
