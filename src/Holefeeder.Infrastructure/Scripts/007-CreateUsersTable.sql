CREATE TABLE IF NOT EXISTS users
(
    id        UUID  NOT NULL PRIMARY KEY,
    inactive  BOOLEAN  NOT NULL
);

CREATE TABLE IF NOT EXISTS user_identities
(
    user_id             UUID           NOT NULL,
    identity_object_id  VARCHAR(255)   NOT NULL,
    inactive            BOOLEAN        NOT NULL,
    PRIMARY KEY (user_id, identity_object_id)
);

CREATE UNIQUE INDEX IF NOT EXISTS identity_object_id_idx ON user_identities (identity_object_id);
