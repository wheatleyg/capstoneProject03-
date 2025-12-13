USE `fun_facts_db`;

-- Insert two genres (space and cats). Uses ON DUPLICATE KEY so running multiple times is safe.
INSERT INTO `main` (GenreName, Description, TableName, Visible)
VALUES
	('Space', 'Interesting space facts', 'SpaceDb', 1),
	('Cats', 'Cute and curious cat facts', 'CatDb', 1)
ON DUPLICATE KEY UPDATE
	GenreName = VALUES(GenreName),
	Description = VALUES(Description),
	Visible = VALUES(Visible);

-- Insert two media rows if they don't already exist.
INSERT INTO `media` (MediaType, Link)
SELECT 'misc', 'https://example.com/space-source'
WHERE NOT EXISTS (SELECT 1 FROM `media` WHERE Link = 'https://example.com/space-source');

INSERT INTO `media` (MediaType, Link)
SELECT 'image', 'https://example.com/cat-image'
WHERE NOT EXISTS (SELECT 1 FROM `media` WHERE Link = 'https://example.com/cat-image');

-- Capture the generated Ids into variables for use when inserting facts.
SELECT Id INTO @space_genre_id FROM `main` WHERE TableName = 'SpaceDb' LIMIT 1;
SELECT Id INTO @cat_genre_id FROM `main` WHERE TableName = 'CatDb' LIMIT 1;

SELECT Id INTO @space_media_id FROM `media` WHERE Link = 'https://example.com/space-source' LIMIT 1;
SELECT Id INTO @cat_media_id FROM `media` WHERE Link = 'https://example.com/cat-image' LIMIT 1;

-- Insert one fact into `space_db` and one into `cat_db`. Use WHERE NOT EXISTS to avoid duplicates.
INSERT INTO `space_db` (GenreId, FactText, SourceId)
SELECT COALESCE(@space_genre_id, 1), 'The Sun contains 99.86% of the mass in the Solar System.', @space_media_id
WHERE NOT EXISTS (
	SELECT 1 FROM `space_db` WHERE FactText = 'The Sun contains 99.86% of the mass in the Solar System.'
);

INSERT INTO `cat_db` (GenreId, FactText, SourceId)
SELECT @cat_genre_id, 'Cats sleep for around 13 to 16 hours a day on average.', @cat_media_id
FROM DUAL
WHERE @cat_genre_id IS NOT NULL
  AND NOT EXISTS (
	SELECT 1 FROM `cat_db` WHERE FactText = 'Cats sleep for around 13 to 16 hours a day on average.'
);

-- Show inserted/existing demo rows for quick verification.
SELECT * FROM `main` WHERE TableName IN ('space_db', 'cat_db');
SELECT * FROM `media` WHERE Link IN ('https://example.com/space-source', 'https://example.com/cat-image');
SELECT * FROM `space_db` LIMIT 5;
SELECT * FROM `cat_db` LIMIT 5;

