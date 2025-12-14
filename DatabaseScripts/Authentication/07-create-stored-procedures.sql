USE `fun_facts_db`;

-- Stored procedure to get fact tags with genre information (joins fact_tags and main)
DROP PROCEDURE IF EXISTS `sp_GetFactTagsWithGenre`;
DELIMITER //
CREATE PROCEDURE `sp_GetFactTagsWithGenre`(IN p_genre_id INT UNSIGNED)
BEGIN
    SELECT 
        ft.Id,
        ft.GenreId,
        ft.AvailableTags,
        ft.CreatedAt,
        ft.UpdatedAt,
        m.GenreName,
        m.Description,
        m.TableName,
        m.Visible
    FROM `fact_tags` ft
    INNER JOIN `main` m ON ft.GenreId = m.Id
    WHERE ft.GenreId = p_genre_id;
END //
DELIMITER ;

-- Stored procedure to get facts with media and genre information (joins space_db, media, and main)
DROP PROCEDURE IF EXISTS `sp_GetSpaceFactsWithDetails`;
DELIMITER //
CREATE PROCEDURE `sp_GetSpaceFactsWithDetails`(IN p_genre_id INT UNSIGNED)
BEGIN
    SELECT 
        s.Id,
        s.GenreId,
        s.FactText,
        s.SourceId,
        s.CreatedAt,
        s.UpdatedAt,
        m.GenreName,
        m.Description,
        m.TableName,
        m.Visible,
        med.MediaType,
        med.Link AS MediaLink,
        med.CreatedAt AS MediaCreatedAt,
        med.UpdatedAt AS MediaUpdatedAt
    FROM `space_db` s
    INNER JOIN `main` m ON s.GenreId = m.Id
    INNER JOIN `media` med ON s.SourceId = med.Id
    WHERE s.GenreId = p_genre_id;
END //
DELIMITER ;

-- Stored procedure to get cat facts with media and genre information (joins cat_db, media, and main)
DROP PROCEDURE IF EXISTS `sp_GetCatFactsWithDetails`;
DELIMITER //
CREATE PROCEDURE `sp_GetCatFactsWithDetails`(IN p_genre_id INT UNSIGNED)
BEGIN
    SELECT 
        c.Id,
        c.GenreId,
        c.FactText,
        c.SourceId,
        c.CreatedAt,
        c.UpdatedAt,
        m.GenreName,
        m.Description,
        m.TableName,
        m.Visible,
        med.MediaType,
        med.Link AS MediaLink,
        med.CreatedAt AS MediaCreatedAt,
        med.UpdatedAt AS MediaUpdatedAt
    FROM `cat_db` c
    INNER JOIN `main` m ON c.GenreId = m.Id
    INNER JOIN `media` med ON c.SourceId = med.Id
    WHERE c.GenreId = p_genre_id;
END //
DELIMITER ;

-- Stored procedure to create fact tags entry
DROP PROCEDURE IF EXISTS `sp_FactTags_Create`;
DELIMITER //
CREATE PROCEDURE `sp_FactTags_Create`(
    IN p_genre_id INT UNSIGNED,
    IN p_available_tags JSON
)
BEGIN
    INSERT INTO `fact_tags` (GenreId, AvailableTags)
    VALUES (p_genre_id, p_available_tags);
    
    SELECT LAST_INSERT_ID() AS NewId;
END //
DELIMITER ;

-- Stored procedure to update fact tags entry
DROP PROCEDURE IF EXISTS `sp_FactTags_Update`;
DELIMITER //
CREATE PROCEDURE `sp_FactTags_Update`(
    IN p_id INT UNSIGNED,
    IN p_genre_id INT UNSIGNED,
    IN p_available_tags JSON
)
BEGIN
    UPDATE `fact_tags`
    SET GenreId = p_genre_id,
        AvailableTags = p_available_tags
    WHERE Id = p_id;
END //
DELIMITER ;

-- Stored procedure to create media entry
DROP PROCEDURE IF EXISTS `sp_Media_Create`;
DELIMITER //
CREATE PROCEDURE `sp_Media_Create`(
    IN p_media_type VARCHAR(10),
    IN p_link VARCHAR(2083)
)
BEGIN
    INSERT INTO `media` (MediaType, Link)
    VALUES (p_media_type, p_link);
    
    SELECT LAST_INSERT_ID() AS NewId;
END //
DELIMITER ;

-- Stored procedure to update media entry
DROP PROCEDURE IF EXISTS `sp_Media_Update`;
DELIMITER //
CREATE PROCEDURE `sp_Media_Update`(
    IN p_id INT UNSIGNED,
    IN p_media_type VARCHAR(10),
    IN p_link VARCHAR(2083)
)
BEGIN
    UPDATE `media`
    SET MediaType = p_media_type,
        Link = p_link
    WHERE Id = p_id;
END //
DELIMITER ;
