
USE `fun_facts_db`;

CREATE TABLE IF NOT EXISTS `fact_tags`(
    `fact_id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Unique identifier for the fact',
    `fact_tags` JSON  NOT NULL COMMENT 'Tag associated with fact',
    PRIMARY KEY (`fact_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
 