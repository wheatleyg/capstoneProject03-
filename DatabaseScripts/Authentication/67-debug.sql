USE `fun_facts_db`;
SHOW TABLES;



SET FOREIGN_KEY_CHECKS = false;
DROP TABLE IF EXISTS `main`;
DROP TABLE IF EXISTS `media`;
DROP TABLE IF EXISTS `space_db`;
DROP TABLE IF EXISTS `fact_tags`;
DROP TABLE IF EXISTS `cat_db`;


SET FOREIGN_KEY_CHECKS = true;
SHOW TABLES;
/* debug for working on multiple different systems. it lets me delete the old tables and replace them with the new ones. :P */