CREATE TABLE IF NOT EXISTS store_items
(
    id      UUID          NOT NULL,
    code    VARCHAR(100)  NOT NULL,
    data    TEXT          NOT NULL,
    user_id UUID          NOT NULL,
    PRIMARY KEY (id)
);

CREATE UNIQUE INDEX IF NOT EXISTS user_id_code_idx ON store_items (user_id, code);
