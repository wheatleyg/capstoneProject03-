
USE `fun_facts_db`;

-- Renamed columns to be clearer
CREATE TABLE IF NOT EXISTS `fact_tags`(
    `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'PK',
    `genre_id` INT UNSIGNED NOT NULL COMMENT 'Links to the main genre table',
    `available_tags` JSON NOT NULL COMMENT 'A JSON array of allowed tags for this genre',
    PRIMARY KEY (`id`),
    FOREIGN KEY (`genre_id`) REFERENCES `main`(`id`) ON DELETE CASCADE
)
 