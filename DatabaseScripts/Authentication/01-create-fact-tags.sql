CREATE TABLE `fact_tags`(
    `fact_id` INT UNSIGNED NOT NULL,
    `fact_tag` VARCHAR(100) NOT NULL COMMENT 'Tag associated with fact',
    PRIMARY KEY (`fact_id`, `fact_tag`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;