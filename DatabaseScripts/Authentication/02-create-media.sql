USE `fun_facts_db`;

CREATE TABLE IF NOT EXISTS `media` (
    `id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'id!',
    `media_type` ENUM('image', 'video', 'audio', 'misc') NOT NULL COMMENT 'The type of media.',
    `link` VARCHAR(2083) NOT NULL COMMENT 'Url link to media. Not sure how ill add files?',
    PRIMARY KEY (`id`)
)

