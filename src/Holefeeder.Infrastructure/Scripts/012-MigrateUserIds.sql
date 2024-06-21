-- Step 1: Generate UUIDs for each distinct user_id from accounts
CREATE TEMPORARY TABLE temp_users AS
SELECT DISTINCT user_id, UUID() as new_id
FROM accounts
GROUP BY user_id;

-- Step 2: Insert these UUIDs into the users table
INSERT INTO users (id, inactive)
SELECT new_id, FALSE
FROM temp_users;

-- Step 3: Insert the user_id from accounts into user_identities
INSERT INTO user_identities (user_id, identity_object_id, inactive)
SELECT new_id, user_id, FALSE
FROM temp_users;

-- Step 4: Update the user_id in accounts to the new UUID
UPDATE accounts
JOIN user_identities ON accounts.user_id = user_identities.identity_object_id
SET accounts.user_id = user_identities.user_id;

ALTER TABLE accounts
    ADD CONSTRAINT fk_account_user
        FOREIGN KEY (user_id) REFERENCES users(id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT;

-- Step 5: Update the user_id in categories to the new UUID
UPDATE categories
JOIN user_identities ON categories.user_id = user_identities.identity_object_id
SET categories.user_id = user_identities.user_id;

ALTER TABLE categories
    ADD CONSTRAINT fk_category_user
        FOREIGN KEY (user_id) REFERENCES users(id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT;

-- Step 6: Update the user_id in cashflows to the new UUID
UPDATE cashflows
JOIN user_identities ON cashflows.user_id = user_identities.identity_object_id
SET cashflows.user_id = user_identities.user_id;

ALTER TABLE cashflows
    ADD CONSTRAINT fk_cashflow_user
        FOREIGN KEY (user_id) REFERENCES users(id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT;

-- Step 7: Update the user_id in transactions to the new UUID
UPDATE transactions
JOIN user_identities ON transactions.user_id = user_identities.identity_object_id
SET transactions.user_id = user_identities.user_id;

ALTER TABLE transactions
    ADD CONSTRAINT fk_transaction_user
        FOREIGN KEY (user_id) REFERENCES users(id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT;

-- Step 8: Update the user_id in store_items to the new UUID
UPDATE store_items
JOIN user_identities ON store_items.user_id = user_identities.identity_object_id
SET store_items.user_id = user_identities.user_id;

ALTER TABLE store_items
    ADD CONSTRAINT fk_store_item_user
        FOREIGN KEY (user_id) REFERENCES users(id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT;
