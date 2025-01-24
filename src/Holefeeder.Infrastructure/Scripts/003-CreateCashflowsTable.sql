CREATE TABLE IF NOT EXISTS cashflows
(
    id             UUID           NOT NULL PRIMARY KEY,
    effective_date DATE           NOT NULL,
    amount         DECIMAL(19, 2) NOT NULL,
    interval_type  VARCHAR(100)   NOT NULL,
    frequency      INT            NOT NULL,
    recurrence     INT            NOT NULL,
    description    TEXT,
    account_id     UUID           NOT NULL,
    category_id    UUID           NOT NULL,
    inactive       BOOLEAN        NOT NULL,
    tags           TEXT,
    user_id        UUID           NOT NULL,
    CONSTRAINT fk_cashflow_account
        FOREIGN KEY (account_id) REFERENCES accounts (id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT,
    CONSTRAINT fk_cashflow_category
        FOREIGN KEY (category_id) REFERENCES categories (id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT
);

CREATE INDEX IF NOT EXISTS user_id_effective_date_idx ON cashflows (user_id, effective_date);
