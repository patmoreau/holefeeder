CREATE TABLE IF NOT EXISTS categories
(
    id            BINARY(16)     NOT NULL PRIMARY KEY,
    type          NVARCHAR(100)  NOT NULL,
    name          NVARCHAR(100)  NOT NULL,
    color         NVARCHAR(25)   NOT NULL,
    budget_amount DECIMAL(19, 2) NOT NULL,
    favorite      BOOL           NOT NULL,
    `system`      BOOL           NOT NULL,
    user_id       BINARY(16)     NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS user_id_name_idx ON categories (user_id, name);
