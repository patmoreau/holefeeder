CREATE TABLE cashflows (
    id BINARY(16) NOT NULL PRIMARY KEY,
    effective_date DATE NOT NULL,
    amount DECIMAL(19,2) NOT NULL,
    interval_type NVARCHAR(100) NOT NULL,
    frequency INT NOT NULL,
    recurrence INT NOT NULL,
    description TINYTEXT,
    account_id BINARY(16) NOT NULL,
    category_id BINARY(16) NOT NULL,
    inactive BOOL NOT NULL,
    tags TEXT,
    user_id BINARY(16) NOT NULL,
    CONSTRAINT fk_cashflow_account
        FOREIGN KEY (account_id) REFERENCES accounts (id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT,
    CONSTRAINT fk_cashflow_category
        FOREIGN KEY (category_id) REFERENCES categories (id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT
);

CREATE INDEX user_id_effective_date_idx ON cashflows (user_id, effective_date);
