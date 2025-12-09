USE `fun_facts_db`;

CREATE TABLE IF NOT EXISTS `media` (
    `Id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'id!',
    `MediaType` ENUM('image', 'video', 'audio', 'misc', 'none') NOT NULL COMMENT 'The type of media.',
    `Link` VARCHAR(2083) NOT NULL COMMENT 'Url link to media. Not sure how ill add files?',
    PRIMARY KEY (`Id`)
)

