
USE `fun_facts_db`;

-- Renamed columns to be clearer
CREATE TABLE IF NOT EXISTS `fact_tags`(
    `Id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'PK',
    `GenreId` INT UNSIGNED NOT NULL COMMENT 'Links to the main genre table',
    `AvailableTags` JSON NOT NULL COMMENT 'A JSON array of allowed tags for this genre',
    PRIMARY KEY (`Id`),
    FOREIGN KEY (`GenreId`) REFERENCES `main`(`Id`) ON DELETE CASCADE
)
 