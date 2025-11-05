CREATE TABLE IF NOT EXISTS `Users` (
    `Id` INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    `CreateDatetime` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `Username` NVARCHAR(64) UNIQUE NOT NULL,
    `EmailAddress` NVARCHAR(128) UNIQUE NOT NULL,
    `PasswordHash` VARBINARY(64) NOT NULL COMMENT 'to fit SHA-512',
    `PasswordSalt` VARBINARY(128),
    `IsDeleted` BIT NOT NULL DEFAULT 0,
    INDEX `ix_users_emailaddress` (`EmailAddress`),
    INDEX `ix_users_username` (`Username`)
);