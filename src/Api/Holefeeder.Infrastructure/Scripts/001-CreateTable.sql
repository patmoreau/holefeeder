CREATE TABLE store_items
(
    id      BINARY(16)    NOT NULL,
    code    NVARCHAR(100) NOT NULL,
    data    TEXT          NOT NULL,
    user_id BINARY(16)    NOT NULL,
    PRIMARY KEY (id)
);

CREATE UNIQUE INDEX user_id_code_idx ON store_items (user_id, code);
