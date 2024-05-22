DELIMITER $$

DROP FUNCTION IF EXISTS BIN_TO_UUID;
CREATE FUNCTION BIN_TO_UUID(uuid_bin BINARY(16))
    RETURNS CHAR(36)
    DETERMINISTIC
BEGIN
    DECLARE uuid_str CHAR(36);

    SET uuid_str = CONCAT(
        HEX(SUBSTRING(uuid_bin, 1, 4)), '-',
        HEX(SUBSTRING(uuid_bin, 5, 2)), '-',
        HEX(SUBSTRING(uuid_bin, 7, 2)), '-',
        HEX(SUBSTRING(uuid_bin, 9, 2)), '-',
        HEX(SUBSTRING(uuid_bin, 11))
                   );

    RETURN uuid_str;
END $$

DELIMITER ;
