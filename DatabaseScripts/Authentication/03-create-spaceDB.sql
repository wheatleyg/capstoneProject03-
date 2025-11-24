USE `fun_facts_db`;

CREATE TABLE IF NOT EXISTS `space_db` (
    `Id` INT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT "id",
    `GenreId` INT UNSIGNED NOT NULL COMMENT 'the genre id',
    `FactText` TEXT NOT NULL COMMENT 'the supposed text of the fact', 
    `SourceId` INT UNSIGNED NOT NULL COMMENT 'source of the supposed fact',
    `CreatedAt` TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'when it was created.',
    `UpdatedAt` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'when it was last updated.',
    PRIMARY KEY (`Id`),
    FOREIGN KEY (`GenreId`) REFERENCES main(`Id`),
    FOREIGN KEY (`SourceId`) REFERENCES media(`Id`)
)