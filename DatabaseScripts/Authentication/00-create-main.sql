CREATE DATABASE IF NOT EXISTS `fun_facts_db`;

USE `fun_facts_db`;

 
    

CREATE TABLE IF NOT EXISTS `main` (
  `Id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `GenreName` VARCHAR(255) NOT NULL COMMENT 'the name of the genre',
  `Description` TEXT DEFAULT NULL COMMENT 'description of the genre',
  `TableName` VARCHAR(64) NOT NULL COMMENT 'the corresponding table name',
  `Visible` TINYINT(1) NOT NULL DEFAULT 1 COMMENT 'is it normally visible? for tables that may be turned off',
  `CreatedAt` TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'when it was created.',
  `UpdatedAt` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'when it was last updated.',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `ux_table_name` (`TableName`)
)
