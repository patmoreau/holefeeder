CREATE TABLE accounts (
    id BINARY(16) NOT NULL PRIMARY KEY,
    type NVARCHAR(100) NOT NULL,
    name NVARCHAR(100) NOT NULL,
    favorite BOOL NOT NULL,
    open_balance DECIMAL(19,2) NOT NULL,
    open_date DATE NOT NULL,
    description TINYTEXT,
    inactive BOOL NOT NULL,
    user_id BINARY(16) NOT NULL
);

CREATE UNIQUE INDEX user_id_name_idx ON accounts (user_id, name);
