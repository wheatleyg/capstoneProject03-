CREATE DATABASE IF NOT EXISTS `fun_facts_db`;


USE `fun_facts_db`;

 
    

CREATE TABLE IF NOT EXISTS `main` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `genre_name` VARCHAR(255) NOT NULL COMMENT 'the name of the genre',
  `description` TEXT DEFAULT NULL COMMENT 'description of the genre',
  `table_name` VARCHAR(64) NOT NULL COMMENT 'the corresponding table name',
  `visible` TINYINT(1) NOT NULL DEFAULT 1 COMMENT 'is it normally visible? for tables that may be turned off',
  PRIMARY KEY (`id`),
  UNIQUE KEY `ux_table_name` (`table_name`)
)
