CREATE TABLE IF NOT EXISTS transactions
(
    id            UUID           NOT NULL PRIMARY KEY,
    date          DATE           NOT NULL,
    amount        DECIMAL(19, 2) NOT NULL,
    description   TEXT,
    account_id    UUID           NOT NULL,
    category_id   UUID           NOT NULL,
    cashflow_id   UUID,
    cashflow_date DATE,
    tags          TEXT,
    user_id       UUID           NOT NULL,
    CONSTRAINT fk_transaction_account
        FOREIGN KEY (account_id) REFERENCES accounts (id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT,
    CONSTRAINT fk_transaction_category
        FOREIGN KEY (category_id) REFERENCES categories (id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT,
    CONSTRAINT fk_transaction_cashflow
        FOREIGN KEY (cashflow_id) REFERENCES cashflows (id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT
);

CREATE INDEX IF NOT EXISTS user_id_date_idx ON transactions (user_id, date);
