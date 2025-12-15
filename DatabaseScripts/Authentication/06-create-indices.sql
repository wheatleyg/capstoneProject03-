USE `fun_facts_db`;

-- Non-clustered index on fact_tags.GenreId for faster lookups
-- (Foreign keys create indices, but explicit index helps with queries filtering by GenreId)
-- Note: MySQL doesn't support IF NOT EXISTS for CREATE INDEX, so this will error if index already exists
-- This is acceptable for rerunnable scripts as the index creation is idempotent in intent
SET @sql = IF((SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
    WHERE table_schema = 'fun_facts_db' AND table_name = 'fact_tags' AND index_name = 'idx_fact_tags_genre_id') = 0,
    'CREATE INDEX `idx_fact_tags_genre_id` ON `fact_tags` (`GenreId`)',
    'SELECT "Index idx_fact_tags_genre_id already exists" AS message');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Non-clustered index on main.GenreName for faster genre lookups
-- Frequently used in WHERE clauses with GenreName
SET @sql = IF((SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
    WHERE table_schema = 'fun_facts_db' AND table_name = 'main' AND index_name = 'idx_main_genre_name') = 0,
    'CREATE INDEX `idx_main_genre_name` ON `main` (`GenreName`)',
    'SELECT "Index idx_main_genre_name already exists" AS message');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Non-clustered index on main.Visible for filtering visible genres
-- Often used in combination with GenreName
SET @sql = IF((SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
    WHERE table_schema = 'fun_facts_db' AND table_name = 'main' AND index_name = 'idx_main_visible') = 0,
    'CREATE INDEX `idx_main_visible` ON `main` (`Visible`)',
    'SELECT "Index idx_main_visible already exists" AS message');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Non-clustered index on media.Link for faster media lookups
-- Used in WHERE NOT EXISTS queries and lookups
SET @sql = IF((SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
    WHERE table_schema = 'fun_facts_db' AND table_name = 'media' AND index_name = 'idx_media_link') = 0,
    'CREATE INDEX `idx_media_link` ON `media` (`Link`)',
    'SELECT "Index idx_media_link already exists" AS message');
PREPARE stmt FROM @sql;
USE `fun_facts_db`;

-- Non-clustered index on fact_tags.GenreId for faster lookups
-- (Foreign keys create indices, but explicit index helps with queries filtering by GenreId)
-- Note: MySQL doesn't support IF NOT EXISTS for CREATE INDEX, so this will error if index already exists
-- This is acceptable for rerunnable scripts as the index creation is idempotent in intent
SET @sql = IF((SELECT COUNT(*)
               FROM INFORMATION_SCHEMA.STATISTICS
               WHERE table_schema = 'fun_facts_db'
                 AND table_name = 'fact_tags'
                 AND index_name = 'idx_fact_tags_genre_id') = 0,
              'CREATE INDEX `idx_fact_tags_genre_id` ON `fact_tags` (`GenreId`)',
              'SELECT "Index idx_fact_tags_genre_id already exists" AS message');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Non-clustered index on main.GenreName for faster genre lookups
-- Frequently used in WHERE clauses with GenreName
SET @sql = IF((SELECT COUNT(*)
               FROM INFORMATION_SCHEMA.STATISTICS
               WHERE table_schema = 'fun_facts_db'
                 AND table_name = 'main'
                 AND index_name = 'idx_main_genre_name') = 0,
              'CREATE INDEX `idx_main_genre_name` ON `main` (`GenreName`)',
              'SELECT "Index idx_main_genre_name already exists" AS message');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Non-clustered index on main.Visible for filtering visible genres
-- Often used in combination with GenreName
SET @sql = IF((SELECT COUNT(*)
               FROM INFORMATION_SCHEMA.STATISTICS
               WHERE table_schema = 'fun_facts_db'
                 AND table_name = 'main'
                 AND index_name = 'idx_main_visible') = 0,
              'CREATE INDEX `idx_main_visible` ON `main` (`Visible`)',
              'SELECT "Index idx_main_visible already exists" AS message');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Non-clustered index on media.Link for faster media lookups
-- Used in WHERE NOT EXISTS queries and lookups
SET @sql = IF((SELECT COUNT(*)
               FROM INFORMATION_SCHEMA.STATISTICS
               WHERE table_schema = 'fun_facts_db'
                 AND table_name = 'media'
                 AND index_name = 'idx_media_link') = 0,
              'CREATE INDEX `idx_media_link` ON `media` (`Link`(768))',
              'SELECT "Index idx_media_link already exists" AS message');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Non-clustered index on space_db.GenreId for faster genre-based queries
SET @sql = IF((SELECT COUNT(*)
               FROM INFORMATION_SCHEMA.STATISTICS
               WHERE table_schema = 'fun_facts_db'
                 AND table_name = 'space_db'
                 AND index_name = 'idx_space_db_genre_id') = 0,
              'CREATE INDEX `idx_space_db_genre_id` ON `space_db` (`GenreId`)',
              'SELECT "Index idx_space_db_genre_id already exists" AS message');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Non-clustered index on cat_db.GenreId for faster genre-based queries
SET @sql = IF((SELECT COUNT(*)
               FROM INFORMATION_SCHEMA.STATISTICS
               WHERE table_schema = 'fun_facts_db'
                 AND table_name = 'cat_db'
                 AND index_name = 'idx_cat_db_genre_id') = 0,
              'CREATE INDEX `idx_cat_db_genre_id` ON `cat_db` (`GenreId`)',
              'SELECT "Index idx_cat_db_genre_id already exists" AS message');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
;
DEALLOCATE PREPARE stmt;

-- Non-clustered index on space_db.GenreId for faster genre-based queries
SET @sql = IF((SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
    WHERE table_schema = 'fun_facts_db' AND table_name = 'space_db' AND index_name = 'idx_space_db_genre_id') = 0,
    'CREATE INDEX `idx_space_db_genre_id` ON `space_db` (`GenreId`)',
    'SELECT "Index idx_space_db_genre_id already exists" AS message');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Non-clustered index on cat_db.GenreId for faster genre-based queries
SET @sql = IF((SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS 
    WHERE table_schema = 'fun_facts_db' AND table_name = 'cat_db' AND index_name = 'idx_cat_db_genre_id') = 0,
    'CREATE INDEX `idx_cat_db_genre_id` ON `cat_db` (`GenreId`)',
    'SELECT "Index idx_cat_db_genre_id already exists" AS message');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
