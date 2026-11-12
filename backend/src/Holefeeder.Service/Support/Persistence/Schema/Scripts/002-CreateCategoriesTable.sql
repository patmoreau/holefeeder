CREATE TABLE IF NOT EXISTS categories
(
    id            UUID           NOT NULL PRIMARY KEY,
    type          VARCHAR(100)   NOT NULL,
    name          VARCHAR(100)   NOT NULL,
    color         VARCHAR(25)    NOT NULL,
    budget_amount DECIMAL(19, 2) NOT NULL,
    favorite      BOOLEAN        NOT NULL,
    system        BOOLEAN        NOT NULL,
    user_id       UUID           NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS user_id_name_idx ON categories (user_id, name);
