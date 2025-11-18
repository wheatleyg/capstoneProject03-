USE `fun_facts_db`;

CREATE TABLE IF NOT EXISTS `cat_db` (
    `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT "id",
    `genre_id` INT UNSIGNED NOT NULL COMMENT 'the gere id',
    `fact_text` TEXT NOT NULL COMMENT 'the supposed text of the fact', 
    `source_id` INT UNSIGNED NOT NULL  COMMENT 'source of the supposed fact',
    `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'when it was created.',
    PRIMARY KEY (`id`),
    FOREIGN KEY (`genre_id`) REFERENCES main(`id`),
    FOREIGN KEY (`source_id`) REFERENCES media(`id`)
);