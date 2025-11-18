USE `fun_facts_db`;

-- Insert two genres (space and cats). Uses ON DUPLICATE KEY so running multiple times is safe.
INSERT INTO `main` (genre_name, description, table_name, visible)
VALUES
	('Space', 'Interesting space facts', 'space_db', 1),
	('Cats', 'Cute and curious cat facts', 'cat_db', 1)
ON DUPLICATE KEY UPDATE
	genre_name = VALUES(genre_name),
	description = VALUES(description),
	visible = VALUES(visible);

-- Insert two media rows if they don't already exist.
INSERT INTO `media` (media_type, link)
SELECT 'misc', 'https://example.com/space-source'
WHERE NOT EXISTS (SELECT 1 FROM `media` WHERE link = 'https://example.com/space-source');

INSERT INTO `media` (media_type, link)
SELECT 'image', 'https://example.com/cat-image'
WHERE NOT EXISTS (SELECT 1 FROM `media` WHERE link = 'https://example.com/cat-image');

-- Capture the generated ids into variables for use when inserting facts.
SELECT id INTO @space_genre_id FROM `main` WHERE table_name = 'space_db' LIMIT 1;
SELECT id INTO @cat_genre_id FROM `main` WHERE table_name = 'cat_db' LIMIT 1;

SELECT id INTO @space_media_id FROM `media` WHERE link = 'https://example.com/space-source' LIMIT 1;
SELECT id INTO @cat_media_id FROM `media` WHERE link = 'https://example.com/cat-image' LIMIT 1;

-- Insert one fact into `space_db` and one into `cat_db`. Use WHERE NOT EXISTS to avoid duplicates.
INSERT INTO `space_db` (genre_id, fact_text, source_id)
SELECT @space_genre_id, 'The Sun contains 99.86% of the mass in the Solar System.', @space_media_id
WHERE NOT EXISTS (
	SELECT 1 FROM `space_db` WHERE fact_text = 'The Sun contains 99.86% of the mass in the Solar System.'
);

INSERT INTO `cat_db` (genre_id, fact_text, source_id)
SELECT @cat_genre_id, 'Cats sleep for around 13 to 16 hours a day on average.', @cat_media_id
WHERE NOT EXISTS (
	SELECT 1 FROM `cat_db` WHERE fact_text = 'Cats sleep for around 13 to 16 hours a day on average.'
);

-- Show inserted/existing demo rows for quick verification.
SELECT * FROM `main` WHERE table_name IN ('space_db', 'cat_db');
SELECT * FROM `media` WHERE link IN ('https://example.com/space-source', 'https://example.com/cat-image');
SELECT * FROM `space_db` LIMIT 5;
SELECT * FROM `cat_db` LIMIT 5;

