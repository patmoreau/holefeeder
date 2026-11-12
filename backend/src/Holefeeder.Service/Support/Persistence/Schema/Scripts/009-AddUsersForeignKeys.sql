ALTER TABLE accounts
    ADD CONSTRAINT fk_account_user
        FOREIGN KEY (user_id) REFERENCES users (id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT;

ALTER TABLE cashflows
    ADD CONSTRAINT fk_cashflow_user
        FOREIGN KEY (user_id) REFERENCES users (id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT;

ALTER TABLE categories
    ADD CONSTRAINT fk_category_user
        FOREIGN KEY (user_id) REFERENCES users (id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT;

ALTER TABLE store_items
    ADD CONSTRAINT fk_store_item_user
        FOREIGN KEY (user_id) REFERENCES users (id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT;

ALTER TABLE transactions
    ADD CONSTRAINT fk_transaction_user
        FOREIGN KEY (user_id) REFERENCES users (id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT;

ALTER TABLE user_identities
    ADD CONSTRAINT fk_user_identities_user
        FOREIGN KEY (user_id) REFERENCES users (id)
            ON DELETE RESTRICT
            ON UPDATE RESTRICT;
