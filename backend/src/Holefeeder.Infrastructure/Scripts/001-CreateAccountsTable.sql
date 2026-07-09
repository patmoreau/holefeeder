CREATE TABLE IF NOT EXISTS accounts
(
    id           UUID           NOT NULL PRIMARY KEY,
    type         VARCHAR(100)   NOT NULL,
    name         VARCHAR(100)   NOT NULL,
    favorite     BOOLEAN        NOT NULL,
    open_balance NUMERIC(19, 2) NOT NULL,
    open_date    DATE           NOT NULL,
    description  TEXT,
    inactive     BOOLEAN        NOT NULL,
    user_id      UUID           NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS user_id_name_idx ON accounts (user_id, name);
